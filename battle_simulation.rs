//! Example battle simulation using the Queen Killer Stand

use queen_killer_stand::{QueenKiller, Target};
use std::thread;
use std::time::Duration;

fn main() {
    println!("🎭 JoJo's Bizarre Adventure: Queen Killer Stand Simulation");

    // Create the Stand user
    let mut kira = QueenKiller::new("Yoshikage Kira".to_string());

    // Create some targets
    let targets = vec![
        Target {
            id: 1,
            name: "Josuke Higashikata".to_string(),
            position: (10.0, 0.0, 5.0),
            health: 100,
            temperature: 37.2, // Higher body temp
            is_alive: true,
        },
        Target {
            id: 2,
            name: "Okuyasu Nijimura".to_string(),
            position: (15.0, 2.0, 3.0),
            health: 100,
            temperature: 36.8,
            is_alive: true,
        },
        Target {
            id: 3,
            name: "Koichi Hirose".to_string(),
            position: (8.0, -1.0, 7.0),
            health: 100,
            temperature: 36.5,
            is_alive: true,
        },
    ];

    // Add targets to the battlefield
    for target in targets {
        println!("➕ Adding target: {}", target.name);
        kira.add_target(target);
    }

    println!("\n🎯 Battle Phase 1: Primary Bomb");
    println!("--------------------------------");
    
    // Place primary bomb on first target
    match kira.place_primary_bomb(1) {
        Ok(_) => println!("✅ Primary bomb placed successfully!"),
        Err(e) => println!("❌ Failed to place bomb: {}", e),
    }

    // Show status
    let status = kira.get_status();
    println!("📊 Status: {} active bombs, {} alive targets", 
             status.active_bomb_count, status.alive_targets);

    // Detonate the bomb
    thread::sleep(Duration::from_secs(1));
    match kira.detonate_bomb(1) {
        Ok(result) => {
            println!("💥 Explosion successful!");
            println!("   - Target: {}", result.target_id);
            println!("   - Damage: {}", result.damage_dealt);
            println!("   - Radius: {}", result.explosion_radius);
        },
        Err(e) => println!("❌ Detonation failed: {}", e),
    }

    println!("\n🔥 Battle Phase 2: Sheer Heart Attack");
    println!("-------------------------------------");

    // Activate Sheer Heart Attack
    match kira.activate_sheer_heart_attack() {
        Ok(_) => println!("✅ Sheer Heart Attack activated!"),
        Err(e) => println!("❌ Failed to activate: {}", e),
    }

    // Simulate some time passing and update
    for i in 1..=3 {
        thread::sleep(Duration::from_millis(500));
        kira.update_sheer_heart_attack();
        println!("🔄 Update cycle {}", i);
    }

    println!("\n⏰ Battle Phase 3: Bites the Dust");
    println!("---------------------------------");

    // Activate Bites the Dust on remaining target
    match kira.activate_bites_the_dust(3) {
        Ok(_) => println!("✅ Bites the Dust activated!"),
        Err(e) => println!("❌ Failed to activate: {}", e),
    }

    // Trigger time loop
    thread::sleep(Duration::from_secs(1));
    match kira.trigger_time_loop() {
        Ok(result) => {
            println!("🔄 Time loop triggered!");
            println!("   - Loop number: {}", result.loop_number);
            println!("   - Reset targets: {:?}", result.reset_targets);
        },
        Err(e) => println!("❌ Time loop failed: {}", e),
    }

    // Final status
    let final_status = kira.get_status();
    println!("\n📈 Final Battle Status:");
    println!("=======================");
    println!("Active bombs: {}", final_status.active_bomb_count);
    println!("Sheer Heart Attack: {}", final_status.sheer_heart_attack_active);
    println!("Bites the Dust: {}", final_status.bites_the_dust_active);
    println!("Time loops: {}", final_status.time_loop_count);
    println!("Alive targets: {}", final_status.alive_targets);
    println!("\n🎭 Simulation complete! What a bizarre adventure!");
}
