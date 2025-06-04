' Data structures for Queen Killer Stand system

Public Enum BombType
    Primary = 1
    SheerHeartAttack = 2
    BitesTheDust = 3
End Enum

Public Class Bomb
    Public Property BombType As BombType
    Public Property TargetId As Integer
    Public Property CreatedAt As DateTime
    Public Property IsActive As Boolean
    Public Property Power As Integer
    Public Property Description As String
End Class

Public Class Target
    Public Property Id As Integer
    Public Property Name As String
    Public Property Position As Position
    Public Property OriginalPosition As Position
    Public Property Health As Integer
    Public Property Temperature As Single
    Public Property IsAlive As Boolean

    Public Sub New()
        Health = 100
        IsAlive = True
        Temperature = 36.5F
    End Sub
End Class

Public Structure Position
    Public X As Single
    Public Y As Single
    Public Z As Single

    Public Sub New(x As Single, y As Single, z As Single)
        Me.X = x
        Me.Y = y
        Me.Z = z
    End Sub
End Structure

' Result classes for each ability
Public Class BombResult
    Public Property Success As Boolean
    Public Property Message As String
    Public Property SoundEffect As String
    Public Property BombPlaced As Bomb
End Class

Public Class ExplosionResult
    Public Property Success As Boolean
    Public Property Message As String
    Public Property TargetId As Integer
    Public Property DamageDealt As Integer
    Public Property ExplosionRadius As Single
    Public Property AffectedTargets As List(Of Integer)
    Public Property SoundEffect As String
End Class

Public Class SheerHeartAttackResult
    Public Property Success As Boolean
    Public Property Message As String
    Public Property TargetId As Integer
    Public Property TargetTemperature As Single
    Public Property SoundEffect As String
End Class

Public Class BitesTheDustResult
    Public Property Success As Boolean
    Public Property Message As String
    Public Property TargetId As Integer
    Public Property TimeLoopReady As Boolean
    Public Property SoundEffect As String
End Class

Public Class TimeLoopResult
    Public Property Success As Boolean
    Public Property Message As String
    Public Property LoopNumber As Integer
    Public Property ResetTargets As List(Of Integer)
    Public Property Reason As String
    Public Property Timestamp As DateTime
    Public Property SoundEffect As String
End Class
