' Queen Killer Stand Implementation in Visual Basic .NET
' Based on Killer Queen from JoJo's Bizarre Adventure

Imports System.Collections.Generic
Imports System.Threading
Imports System.Windows.Forms

Public Class QueenKillerStand
    Private _userName As String
    Private _standName As String = "Queen Killer"
    Private _activeBombs As New Dictionary(Of Integer, Bomb)
    Private _targets As New Dictionary(Of Integer, Target)
    Private _sheerHeartAttackActive As Boolean = False
    Private _bitesTheDustTarget As Integer? = Nothing
    Private _timeLoopCount As Integer = 0
    Private Const MAX_PRIMARY_BOMBS As Integer = 1

    Public Sub New(userName As String)
        _userName = userName
    End Sub

    ' ==========================================
    ' SKILL 1: PRIMARY BOMB - "First Bomb"
    ' ==========================================
    ''' <summary>
    ''' First Bomb: Turns any object into a bomb that can be detonated at will
    ''' - Only one primary bomb can exist at a time
    ''' - Completely destroys the target and creates an explosion
    ''' - Activated by pressing thumb down (signature gesture)
    ''' </summary>
    Public Function PlacePrimaryBomb(targetId As Integer) As BombResult
        Try
            ' Check if primary bomb already exists
            If HasActivePrimaryBomb() Then
                Return New BombResult With {
                    .Success = False,
                    .Message = "❌ Primary bomb already active! Detonate it first.",
                    .SoundEffect = "error"
                }
            End If

            ' Validate target exists
            If Not _targets.ContainsKey(targetId) Then
                Return New BombResult With {
                    .Success = False,
                    .Message = "❌ Target not found!",
                    .SoundEffect = "error"
                }
            End If

            ' Create and place the bomb
            Dim newBomb As New Bomb With {
                .BombType = BombType.Primary,
                .TargetId = targetId,
                .CreatedAt = DateTime.Now,
                .IsActive = True,
                .Power = 100,
                .Description = "Primary explosive device"
            }

            _activeBombs.Add(targetId, newBomb)

            ' Visual and audio feedback
            PlaySoundEffect("bomb_place")
            ShowVisualEffect("✨ *CLICK* ✨")

            Return New BombResult With {
                .Success = True,
                .Message = $"💣 Primary bomb placed on target {targetId}!",
                .SoundEffect = "bomb_place",
                .BombPlaced = newBomb
            }

        Catch ex As Exception
            Return New BombResult With {
                .Success = False,
                .Message = $"❌ Error placing bomb: {ex.Message}",
                .SoundEffect = "error"
            }
        End Try
    End Function

    ''' <summary>
    ''' Detonate Primary Bomb with signature thumb press gesture
    ''' </summary>
    Public Function DetonatePrimaryBomb(targetId As Integer) As ExplosionResult
        Try
            If Not _activeBombs.ContainsKey(targetId) Then
                Return New ExplosionResult With {
                    .Success = False,
                    .Message = "❌ No bomb found on target!"
                }
            End If

            Dim bomb = _activeBombs(targetId)
            If bomb.BombType <> BombType.Primary Then
                Return New ExplosionResult With {
                    .Success = False,
                    .Message = "❌ Target doesn't have a primary bomb!"
                }
            End If

            ' Remove bomb and destroy target
            _activeBombs.Remove(targetId)
            Dim target = _targets(targetId)
            target.Health = 0
            target.IsAlive = False

            ' Calculate explosion effects
            Dim affectedTargets = GetTargetsInRadius(target.Position, 5.0F)
            
            ' Visual effects
            PlaySoundEffect("explosion")
            ShowVisualEffect("💥💥💥 BOOOOM! 💥💥💥")
            ShowVisualEffect($"🎯 Target {target.Name} completely destroyed!")

            Return New ExplosionResult With {
                .Success = True,
                .Message = $"💥 PRIMARY BOMB DETONATED! Target {targetId} destroyed!",
                .TargetId = targetId,
                .DamageDealt = bomb.Power,
                .ExplosionRadius = 5.0F,
                .AffectedTargets = affectedTargets,
                .SoundEffect = "explosion"
            }

        Catch ex As Exception
            Return New ExplosionResult With {
                .Success = False,
                .Message = $"❌ Detonation failed: {ex.Message}"
            }
        End Try
    End Function

    ' ==========================================
    ' SKILL 2: SHEER HEART ATTACK - "Second Bomb"
    ' ==========================================
    ''' <summary>
    ''' Sheer Heart Attack: Autonomous heat-seeking bomb tank
    ''' - Automatically tracks the hottest target
    ''' - Cannot be recalled once activated
    ''' - Moves independently and explodes on contact
    ''' - Indestructible and relentless
    ''' </summary>
    Public Function ActivateSheerHeartAttack() As SheerHeartAttackResult
        Try
            If _sheerHeartAttackActive Then
                Return New SheerHeartAttackResult With {
                    .Success = False,
                    .Message = "❌ Sheer Heart Attack already active!"
                }
            End If

            ' Find the hottest target
            Dim hottestTarget = FindHottestTarget()
            If hottestTarget Is Nothing Then
                Return New SheerHeartAttackResult With {
                    .Success = False,
                    .Message = "❌ No valid targets for Sheer Heart Attack!"
                }
            End If

            ' Create autonomous bomb
            Dim shaBomb As New Bomb With {
                .BombType = BombType.SheerHeartAttack,
                .TargetId = hottestTarget.Id,
                .CreatedAt = DateTime.Now,
                .IsActive = True,
                .Power = 80,
                .Description = "Autonomous heat-seeking tank bomb"
            }

            _activeBombs.Add(hottestTarget.Id, shaBomb)
            _sheerHeartAttackActive = True

            ' Dramatic activation
            PlaySoundEffect("sheer_heart_attack")
            ShowVisualEffect("🔥🔥🔥 SHEER HEART ATTACK! 🔥🔥🔥")
            ShowVisualEffect($"🎯 Targeting hottest enemy: {hottestTarget.Name} ({hottestTarget.Temperature}°C)")
            ShowVisualEffect("⚠️ WARNING: Autonomous bomb cannot be recalled!")

            ' Start tracking thread
            StartSheerHeartAttackTracking()

            Return New SheerHeartAttackResult With {
                .Success = True,
                .Message = "🔥 SHEER HEART ATTACK ACTIVATED!",
                .TargetId = hottestTarget.Id,
                .TargetTemperature = hottestTarget.Temperature,
                .SoundEffect = "sheer_heart_attack"
            }

        Catch ex As Exception
            Return New SheerHeartAttackResult With {
                .Success = False,
                .Message = $"❌ Activation failed: {ex.Message}"
            }
        End Try
    End Function

    ''' <summary>
    ''' Sheer Heart Attack tracking and retargeting logic
    ''' </summary>
    Private Sub StartSheerHeartAttackTracking()
        Dim trackingThread As New Thread(Sub()
            While _sheerHeartAttackActive
                Try
                    ' Check if current target is still alive
                    Dim currentBomb = _activeBombs.Values.FirstOrDefault(Function(b) b.BombType = BombType.SheerHeartAttack)
                    If currentBomb IsNot Nothing Then
                        Dim currentTarget = _targets(currentBomb.TargetId)
                        
                        If Not currentTarget.IsAlive Then
                            ' Retarget to new hottest enemy
                            Dim newTarget = FindHottestTarget()
                            If newTarget IsNot Nothing Then
                                _activeBombs.Remove(currentBomb.TargetId)
                                currentBomb.TargetId = newTarget.Id
                                _activeBombs.Add(newTarget.Id, currentBomb)
                                
                                ShowVisualEffect($"🎯 Sheer Heart Attack retargeting: {newTarget.Name}")
                            Else
                                _sheerHeartAttackActive = False
                                ShowVisualEffect("🔥 Sheer Heart Attack deactivated - no targets")
                            End If
                        End If
                    End If
                    
                    Thread.Sleep(1000) ' Check every second
                Catch ex As Exception
                    Console.WriteLine($"Tracking error: {ex.Message}")
                End Try
            End While
        End Sub)
        
        trackingThread.IsBackground = True
        trackingThread.Start()
    End Sub

    ' ==========================================
    ' SKILL 3: BITES THE DUST - "Third Bomb"
    ' ==========================================
    ''' <summary>
    ''' Bites the Dust: Time loop bomb that resets reality
    ''' - Plants bomb inside a person's eye
    ''' - If someone learns Kira's identity, time resets to 1 hour ago
    ''' - Only the bomb carrier retains memories
    ''' - Can create multiple time loops
    ''' - Ultimate defensive ability
    ''' </summary>
    Public Function ActivateBitesTheDust(targetId As Integer) As BitesTheDustResult
        Try
            If _bitesTheDustTarget.HasValue Then
                Return New BitesTheDustResult With {
                    .Success = False,
                    .Message = "❌ Bites the Dust already active!"
                }
            End If

            If Not _targets.ContainsKey(targetId) Then
                Return New BitesTheDustResult With {
                    .Success = False,
                    .Message = "❌ Target not found!"
                }
            End If

            ' Plant the time bomb
            Dim timeBomb As New Bomb With {
                .BombType = BombType.BitesTheDust,
                .TargetId = targetId,
                .CreatedAt = DateTime.Now,
                .IsActive = True,
                .Power = 200,
                .Description = "Time manipulation bomb planted in eye"
            }

            _activeBombs.Add(targetId, timeBomb)
            _bitesTheDustTarget = targetId

            ' Dramatic activation sequence
            PlaySoundEffect("bites_the_dust")
            ShowVisualEffect("⏰⏰⏰ BITES THE DUST! ⏰⏰⏰")
            ShowVisualEffect($"👁️ Time bomb planted in {_targets(targetId).Name}'s eye!")
            ShowVisualEffect("🔄 Reality manipulation active...")
            ShowVisualEffect("⚠️ If identity is discovered, time will reset!")

            Return New BitesTheDustResult With {
                .Success = True,
                .Message = "⏰ BITES THE DUST ACTIVATED!",
                .TargetId = targetId,
                .TimeLoopReady = True,
                .SoundEffect = "bites_the_dust"
            }

        Catch ex As Exception
            Return New BitesTheDustResult With {
                .Success = False,
                .Message = $"❌ Activation failed: {ex.Message}"
            }
        End Try
    End Function

    ''' <summary>
    ''' Trigger time loop when identity is discovered
    ''' </summary>
    Public Function TriggerTimeLoop(reason As String) As TimeLoopResult
        Try
            If Not _bitesTheDustTarget.HasValue Then
                Return New TimeLoopResult With {
                    .Success = False,
                    .Message = "❌ Bites the Dust not active!"
                }
            End If

            _timeLoopCount += 1
            
            ' Reset all targets except the bomb carrier
            Dim resetTargets As New List(Of Integer)
            For Each kvp In _targets
                If kvp.Key <> _bitesTheDustTarget.Value Then
                    kvp.Value.Health = 100
                    kvp.Value.IsAlive = True
                    kvp.Value.Position = kvp.Value.OriginalPosition ' Reset position
                    resetTargets.Add(kvp.Key)
                End If
            Next

            ' Clear other bombs except Bites the Dust
            Dim bitesTheDustBomb = _activeBombs(_bitesTheDustTarget.Value)
            _activeBombs.Clear()
            _activeBombs.Add(_bitesTheDustTarget.Value, bitesTheDustBomb)
            _sheerHeartAttackActive = False

            ' Dramatic time reset sequence
            PlaySoundEffect("time_loop")
            ShowVisualEffect("🌀🌀🌀 TIME IS REWINDING! 🌀🌀🌀")
            ShowVisualEffect($"⏰ Loop #{_timeLoopCount}: {reason}")
            ShowVisualEffect("🔄 Reality has been reset to 1 hour ago!")
            ShowVisualEffect($"👁️ Only {_targets(_bitesTheDustTarget.Value).Name} remembers...")

            Return New TimeLoopResult With {
                .Success = True,
                .Message = $"🔄 TIME LOOP #{_timeLoopCount} ACTIVATED!",
                .LoopNumber = _timeLoopCount,
                .ResetTargets = resetTargets,
                .Reason = reason,
                .Timestamp = DateTime.Now,
                .SoundEffect = "time_loop"
            }

        Catch ex As Exception
            Return New TimeLoopResult With {
                .Success = False,
                .Message = $"❌ Time loop failed: {ex.Message}"
            }
        End Try
    End Function

    ' ==========================================
    ' HELPER METHODS AND UTILITIES
    ' ==========================================

    Private Function HasActivePrimaryBomb() As Boolean
        Return _activeBombs.Values.Any(Function(b) b.BombType = BombType.Primary)
    End Function

    Private Function FindHottestTarget() As Target
        Return _targets.Values.Where(Function(t) t.IsAlive).
               OrderByDescending(Function(t) t.Temperature).
               FirstOrDefault()
    End Function

    Private Function GetTargetsInRadius(center As Position, radius As Single) As List(Of Integer)
        Dim result As New List(Of Integer)
        For Each kvp In _targets
            Dim distance = CalculateDistance(center, kvp.Value.Position)
            If distance <= radius AndAlso kvp.Value.IsAlive Then
                result.Add(kvp.Key)
            End If
        Next
        Return result
    End Function

    Private Function CalculateDistance(pos1 As Position, pos2 As Position) As Single
        Return Math.Sqrt((pos1.X - pos2.X) ^ 2 + (pos1.Y - pos2.Y) ^ 2 + (pos1.Z - pos2.Z) ^ 2)
    End Function

    Private Sub PlaySoundEffect(effect As String)
        Select Case effect
            Case "bomb_place"
                Console.WriteLine("🔊 *Click* - Bomb armed")
            Case "explosion"
                Console.WriteLine("🔊 💥 BOOOOM! 💥")
            Case "sheer_heart_attack"
                Console.WriteLine("🔊 *Mechanical whirring* SHEER HEART ATTACK!")
            Case "bites_the_dust"
                Console.WriteLine("🔊 ⏰ *Time reversal sound* BITES THE DUST!")
            Case "time_loop"
                Console.WriteLine("🔊 🌀 *Reality warping sounds* 🌀")
            Case "error"
                Console.WriteLine("🔊 ❌ *Error sound*")
        End Select
    End Sub

    Private Sub ShowVisualEffect(message As String)
        Console.WriteLine(message)
        ' In a real application, this would trigger visual effects
    End Sub

    ' Public properties for status checking
    Public ReadOnly Property StandName As String
        Get
            Return _standName
        End Get
    End Property

    Public ReadOnly Property UserName As String
        Get
            Return _userName
        End Get
    End Property

    Public ReadOnly Property ActiveBombCount As Integer
        Get
            Return _activeBombs.Count
        End Get
    End Property

    Public ReadOnly Property IsSheerHeartAttackActive As Boolean
        Get
            Return _sheerHeartAttackActive
        End Get
    End Property

    Public ReadOnly Property IsBitesTheDustActive As Boolean
        Get
            Return _bitesTheDustTarget.HasValue
        End Get
    End Property

    Public ReadOnly Property TimeLoopCount As Integer
        Get
            Return _timeLoopCount
        End Get
    End Property

    Public Sub AddTarget(target As Target)
        _targets.Add(target.Id, target)
    End Sub
End Class
