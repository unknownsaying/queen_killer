' Battle simulation demonstrating all three Queen Killer abilities

Imports System.Threading

Module BattleSimulation
    Sub Main()
        Console.WriteLine("🎭 JoJo's Bizarre Adventure: Queen Killer Stand Demo")

        ' Create the Stand user
        Dim kira As New QueenKillerStand("Yoshikage Kira")

        ' Create targets
        Dim targets As New List(Of Target) From {
            New Target With {
                .Id = 1,
                .Name = "Josuke Higashikata",
                .Position = New Position(10, 0, 5),
                .OriginalPosition = New Position(10, 0, 5),
                .Temperature = 37.2F
            },
            New Target With {
                .Id = 2,
                .Name = "Okuyasu Nijimura",
                .Position = New Position(15, 2, 3),
                .OriginalPosition = New Position(15, 2, 3),
                .Temperature = 36.8F
            },
            New Target With {
                .Id = 3,
                .Name = "Koichi Hirose",
                .Position = New Position(8, -1, 7),
                .OriginalPosition = New Position(8, -1, 7),
                .Temperature = 36.5F
            }
        }

        ' Add targets to battlefield
        For Each target In targets
            kira.AddTarget(target)
            Console.WriteLine($"➕ Added target: {target.Name}")
        Next
        Console.WriteLine()

        ' ==========================================
        ' DEMONSTRATE SKILL 1: PRIMARY BOMB
        ' ==========================================
        Console.WriteLine("💣 SKILL 1 DEMONSTRATION: PRIMARY BOMB")
        Console.WriteLine("======================================")
        Console.WriteLine("📖 Description: Turns any object into a bomb")
        Console.WriteLine("   - Only one can exist at a time")
        Console.WriteLine("   - Completely destroys target")
        Console.WriteLine("   - Activated by thumb press gesture")
        Console.WriteLine()

        ' Place primary bomb
        Dim bombResult = kira.PlacePrimaryBomb(1)
        Console.WriteLine(bombResult.Message)
        Thread.Sleep(2000)

        ' Detonate primary bomb
        Console.WriteLine("👍 *Kira presses thumb down*")
        Dim explosionResult = kira.DetonatePrimaryBomb(1)
        Console.WriteLine(explosionResult.Message)
        Console.WriteLine()
        Thread.Sleep(3000)

        ' ==========================================
        ' DEMONSTRATE SKILL 2: SHEER HEART ATTACK
        ' ==========================================
        Console.WriteLine("🔥 SKILL 2 DEMONSTRATION: SHEER HEART ATTACK")
        Console.WriteLine("============================================")
        Console.WriteLine("📖 Description: Autonomous heat-seeking bomb")
        Console.WriteLine("   - Tracks hottest target automatically")
        Console.WriteLine("   - Cannot be recalled once activated")
        Console.WriteLine("   - Indestructible tank-like bomb")
        Console.WriteLine()

        Dim shaResult = kira.ActivateSheerHeartAttack()
        Console.WriteLine(shaResult.Message)
        
        ' Simulate tracking
        For i As Integer = 1 To 3
            Thread.Sleep(1500)
            Console.WriteLine($"🎯 Sheer Heart Attack tracking... ({i}/3)")
        Next
        Console.WriteLine()
        Thread.Sleep(2000)

        ' ==========================================
        ' DEMONSTRATE SKILL 3: BITES THE DUST
        ' ==========================================
        Console.WriteLine("⏰ SKILL 3 DEMONSTRATION: BITES THE DUST")
        Console.WriteLine("========================================")
        Console.WriteLine("📖 Description: Time loop bomb")
        Console.WriteLine("   - Plants bomb in target's eye")
        Console.WriteLine("   - Resets time when identity discovered")
        Console.WriteLine("   - Ultimate defensive ability")
        Console.WriteLine()

        Dim bitesResult = kira.ActivateBitesTheDust(3)
        Console.WriteLine(bitesResult.Message)
        Thread.Sleep(2000)

        ' Simulate identity discovery
        Console.WriteLine("🕵️ Someone discovered Kira's identity!")
        Thread.Sleep(1000)
        
        Dim timeLoopResult = kira.TriggerTimeLoop("Identity discovered by investigator")
        Console.WriteLine(timeLoopResult.Message)
        Console.WriteLine()
        Thread.Sleep(3000)

        ' ==========================================
        ' FINAL STATUS
        ' ==========================================
        Console.WriteLine("📊 FINAL STAND STATUS")
        Console.WriteLine("====================")
        Console.WriteLine($"Stand User: {kira.UserName}")
        Console.WriteLine($"Stand Name: {kira.StandName}")
        Console.WriteLine($"Active Bombs: {kira.ActiveBombCount}")
        Console.WriteLine($"Sheer Heart Attack: {If(kira.IsSheerHeartAttackActive, "Active", "Inactive")}")
        Console.WriteLine($"Bites the Dust: {If(kira.IsBitesTheDustActive, "Active", "Inactive")}")
        Console.WriteLine($"Time Loops: {kira.TimeLoopCount}")
        Console.WriteLine()

        Console.WriteLine("🎭 What a bizarre adventure! Press any key to exit...")
        Console.ReadKey()
    End Sub
End Module
