//! Queen Killer Stand Framework
//! A Rust implementation of a JoJo Stand system with Killer Queen-inspired abilities

use std::collections::HashMap;
use std::time::{Duration, Instant};

/// Represents the state of a bomb placed by Queen Killer
#[derive(Debug, Clone)]
pub enum BombType {
    Primary,    // First Bomb - turns objects into bombs
    Secondary,  // Sheer Heart Attack - heat-seeking bomb
    Tertiary,   // Bites the Dust - time loop bomb
}

/// Represents an object that has been turned into a bomb
#[derive(Debug, Clone)]
pub struct Bomb {
    pub bomb_type: BombType,
    pub target_id: u32,
    pub created_at: Instant,
    pub is_active: bool,
    pub power: u32,
}

/// Represents a target that can be affected by the Stand
#[derive(Debug, Clone)]
pub struct Target {
    pub id: u32,
    pub name: String,
    pub position: (f32, f32, f32),
    pub health: i32,
    pub temperature: f32, // For Sheer Heart Attack targeting
    pub is_alive: bool,
}

/// The main Queen Killer Stand structure
#[derive(Debug)]
pub struct QueenKiller {
    pub user_name: String,
    pub stand_name: String,
    pub active_bombs: HashMap<u32, Bomb>,
    pub targets: HashMap<u32, Target>,
    pub max_primary_bombs: usize,
    pub sheer_heart_attack_active: bool,
    pub bites_the_dust_target: Option<u32>,
    pub time_loop_count: u32,
}

impl QueenKiller {
    /// Creates a new Queen Killer Stand instance
    pub fn new(user_name: String) -> Self {
        Self {
            user_name,
            stand_name: "Queen Killer".to_string(),
            active_bombs: HashMap::new(),
            targets: HashMap::new(),
            max_primary_bombs: 1, // Killer Queen can only have one primary bomb at a time
            sheer_heart_attack_active: false,
            bites_the_dust_target: None,
            time_loop_count: 0,
        }
    }

    /// First Bomb: Turn an object into a bomb
    pub fn place_primary_bomb(&mut self, target_id: u32) -> Result<(), String> {
        // Check if we already have a primary bomb active
        if self.active_bombs.values().any(|b| matches!(b.bomb_type, BombType::Primary)) {
            return Err("Primary bomb already active. Detonate first.".to_string());
        }

        if !self.targets.contains_key(&target_id) {
            return Err("Target not found".to_string());
        }

        let bomb = Bomb {
            bomb_type: BombType::Primary,
            target_id,
            created_at: Instant::now(),
            is_active: true,
            power: 100,
        };

        self.active_bombs.insert(target_id, bomb);
        println!("Primary bomb placed on target {}", target_id);
        Ok(())
    }

    /// Detonate a specific bomb
    pub fn detonate_bomb(&mut self, target_id: u32) -> Result<ExplosionResult, String> {
        if let Some(bomb) = self.active_bombs.remove(&target_id) {
            if let Some(target) = self.targets.get_mut(&target_id) {
                target.health = 0;
                target.is_alive = false;
                
                let result = ExplosionResult {
                    target_id,
                    damage_dealt: bomb.power,
                    explosion_radius: 5.0,
                    affected_targets: self.get_targets_in_radius(target.position, 5.0),
                };

                println!("💥 BOOM! Target {} destroyed!", target_id);
                return Ok(result);
            }
        }
        Err("No bomb found on target".to_string())
    }

    /// Second Bomb: Sheer Heart Attack - autonomous heat-seeking bomb
    pub fn activate_sheer_heart_attack(&mut self) -> Result<(), String> {
        if self.sheer_heart_attack_active {
            return Err("Sheer Heart Attack already active".to_string());
        }

        // Find the hottest target
        let hottest_target = self.targets
            .values()
            .filter(|t| t.is_alive)
            .max_by(|a, b| a.temperature.partial_cmp(&b.temperature).unwrap());

        if let Some(target) = hottest_target {
            let bomb = Bomb {
                bomb_type: BombType::Secondary,
                target_id: target.id,
                created_at: Instant::now(),
                is_active: true,
                power: 80,
            };

            self.active_bombs.insert(target.id, bomb);
            self.sheer_heart_attack_active = true;
            println!("🔥 Sheer Heart Attack activated! Targeting hottest enemy (ID: {})", target.id);
            Ok(())
        } else {
            Err("No valid targets for Sheer Heart Attack".to_string())
        }
    }

    /// Third Bomb: Bites the Dust - time loop ability
    pub fn activate_bites_the_dust(&mut self, target_id: u32) -> Result<(), String> {
        if self.bites_the_dust_target.is_some() {
            return Err("Bites the Dust already active".to_string());
        }

        if !self.targets.contains_key(&target_id) {
            return Err("Target not found".to_string());
        }

        let bomb = Bomb {
            bomb_type: BombType::Tertiary,
            target_id,
            created_at: Instant::now(),
            is_active: true,
            power: 200,
        };

        self.active_bombs.insert(target_id, bomb);
        self.bites_the_dust_target = Some(target_id);
        println!("⏰ Bites the Dust activated on target {}!", target_id);
        Ok(())
    }

    /// Trigger time loop (Bites the Dust effect)
    pub fn trigger_time_loop(&mut self) -> Result<TimeLoopResult, String> {
        if let Some(target_id) = self.bites_the_dust_target {
            self.time_loop_count += 1;
            
            // Reset all targets to previous state (simplified)
            for target in self.targets.values_mut() {
                if target.id != target_id {
                    target.health = 100; // Reset health
                    target.is_alive = true;
                }
            }

            // Clear other bombs except Bites the Dust
            self.active_bombs.retain(|_, bomb| matches!(bomb.bomb_type, BombType::Tertiary));
            self.sheer_heart_attack_active = false;

            let result = TimeLoopResult {
                loop_number: self.time_loop_count,
                reset_targets: self.targets.keys().cloned().collect(),
                timestamp: Instant::now(),
            };

            println!("🔄 Time loop #{} activated! Reality reset!", self.time_loop_count);
            Ok(result)
        } else {
            Err("Bites the Dust not active".to_string())
        }
    }

    /// Add a target to the battlefield
    pub fn add_target(&mut self, target: Target) {
        self.targets.insert(target.id, target);
    }

    /// Get targets within explosion radius
    fn get_targets_in_radius(&self, center: (f32, f32, f32), radius: f32) -> Vec<u32> {
        self.targets
            .values()
            .filter(|target| {
                let distance = ((target.position.0 - center.0).powi(2) +
                               (target.position.1 - center.1).powi(2) +
                               (target.position.2 - center.2).powi(2)).sqrt();
                distance <= radius && target.is_alive
            })
            .map(|target| target.id)
            .collect()
    }

    /// Update Sheer Heart Attack movement (call this in game loop)
    pub fn update_sheer_heart_attack(&mut self) {
        if !self.sheer_heart_attack_active {
            return;
        }

        // Find new hottest target if current target is dead
        let current_target_alive = self.active_bombs
            .values()
            .find(|b| matches!(b.bomb_type, BombType::Secondary))
            .and_then(|bomb| self.targets.get(&bomb.target_id))
            .map(|target| target.is_alive)
            .unwrap_or(false);

        if !current_target_alive {
            // Retarget to hottest enemy
            if let Some(new_target) = self.targets
                .values()
                .filter(|t| t.is_alive)
                .max_by(|a, b| a.temperature.partial_cmp(&b.temperature).unwrap()) {
                
                // Remove old bomb and create new one
                self.active_bombs.retain(|_, bomb| !matches!(bomb.bomb_type, BombType::Secondary));
                
                let bomb = Bomb {
                    bomb_type: BombType::Secondary,
                    target_id: new_target.id,
                    created_at: Instant::now(),
                    is_active: true,
                    power: 80,
                };
                
                self.active_bombs.insert(new_target.id, bomb);
                println!("🎯 Sheer Heart Attack retargeting to {}", new_target.id);
            } else {
                self.sheer_heart_attack_active = false;
                println!("Sheer Heart Attack deactivated - no targets");
            }
        }
    }

    /// Get current stand status
    pub fn get_status(&self) -> StandStatus {
        StandStatus {
            active_bomb_count: self.active_bombs.len(),
            sheer_heart_attack_active: self.sheer_heart_attack_active,
            bites_the_dust_active: self.bites_the_dust_target.is_some(),
            time_loop_count: self.time_loop_count,
            alive_targets: self.targets.values().filter(|t| t.is_alive).count(),
        }
    }
}

/// Result of an explosion
#[derive(Debug)]
pub struct ExplosionResult {
    pub target_id: u32,
    pub damage_dealt: u32,
    pub explosion_radius: f32,
    pub affected_targets: Vec<u32>,
}

/// Result of a time loop activation
#[derive(Debug)]
pub struct TimeLoopResult {
    pub loop_number: u32,
    pub reset_targets: Vec<u32>,
    pub timestamp: Instant,
}

/// Current status of the Stand
#[derive(Debug)]
pub struct StandStatus {
    pub active_bomb_count: usize,
    pub sheer_heart_attack_active: bool,
    pub bites_the_dust_active: bool,
    pub time_loop_count: u32,
    pub alive_targets: usize,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_queen_killer_creation() {
        let stand = QueenKiller::new("Yoshikage Kira".to_string());
        assert_eq!(stand.user_name, "Yoshikage Kira");
        assert_eq!(stand.stand_name, "Queen Killer");
    }

    #[test]
    fn test_primary_bomb_placement() {
        let mut stand = QueenKiller::new("Kira".to_string());
        let target = Target {
            id: 1,
            name: "Enemy".to_string(),
            position: (0.0, 0.0, 0.0),
            health: 100,
            temperature: 36.5,
            is_alive: true,
        };
        
        stand.add_target(target);
        assert!(stand.place_primary_bomb(1).is_ok());
        assert!(stand.active_bombs.contains_key(&1));
    }
}
