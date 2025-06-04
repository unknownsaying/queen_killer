//! Additional abilities and traits for the Queen Killer Stand

use crate::{Bomb, Target, BombType};
use std::time::Instant;

/// Trait for Stand abilities
pub trait StandAbility {
    fn activate(&mut self, target_id: Option<u32>) -> Result<String, String>;
    fn deactivate(&mut self) -> Result<String, String>;
    fn is_active(&self) -> bool;
}

/// Advanced bomb manipulation abilities
pub trait BombManipulation {
    fn set_bomb_timer(&mut self, target_id: u32, delay_seconds: u64) -> Result<(), String>;
    fn create_chain_explosion(&mut self, epicenter: u32, chain_length: u32) -> Result<Vec<u32>, String>;
    fn bomb_defusal(&mut self, target_id: u32) -> Result<(), String>;
}

/// Killer Queen's signature hand gestures and activation methods
#[derive(Debug, Clone)]
pub enum ActivationGesture {
    ThumbPress,     // Primary bomb detonation
    FingerSnap,     // Sheer Heart Attack
    HandClench,     // Bites the Dust
}

/// Enhanced bomb with timer functionality
#[derive(Debug, Clone)]
pub struct TimedBomb {
    pub base_bomb: Bomb,
    pub timer_duration: Option<std::time::Duration>,
    pub auto_detonate: bool,
}

impl TimedBomb {
    pub fn new(bomb_type: BombType, target_id: u32, power: u32) -> Self {
        Self {
            base_bomb: Bomb {
                bomb_type,
                target_id,
                created_at: Instant::now(),
                is_active: true,
                power,
            },
            timer_duration: None,
            auto_detonate: false,
        }
    }

    pub fn with_timer(mut self, duration: std::time::Duration) -> Self {
        self.timer_duration = Some(duration);
        self.auto_detonate = true;
        self
    }

    pub fn should_detonate(&self) -> bool {
        if let Some(duration) = self.timer_duration {
            self.base_bomb.created_at.elapsed() >= duration && self.auto_detonate
        } else {
            false
        }
    }
}

/// Stand stats following JoJo's stat system
#[derive(Debug, Clone)]
pub struct StandStats {
    pub destructive_power: u8,  // A-E ranking (5-1)
    pub speed: u8,
    pub range: u8,
    pub durability: u8,
    pub precision: u8,
    pub development_potential: u8,
}

impl Default for StandStats {
    fn default() -> Self {
        // Killer Queen's canonical stats
        Self {
            destructive_power: 5, // A
            speed: 4,             // B
            range: 2,             // D (close-range)
            durability: 4,        // B
            precision: 4,         // B
            development_potential: 5, // A (due to evolution)
        }
    }
}

/// Environmental interaction system
#[derive(Debug, Clone)]
pub struct Environment {
    pub objects: Vec<EnvironmentObject>,
    pub temperature: f32,
    pub humidity: f32,
}

#[derive(Debug, Clone)]
pub struct EnvironmentObject {
    pub id: u32,
    pub name: String,
    pub position: (f32, f32, f32),
    pub can_be_bomb: bool,
    pub material_type: MaterialType,
}

#[derive(Debug, Clone)]
pub enum MaterialType {
    Organic,
    Metal,
    Plastic,
    Glass,
    Stone,
}

/// Sound effects for immersion
pub fn play_sound_effect(effect: &str) {
    match effect {
        "bomb_place" => println!("🔊 *Click* - Bomb armed"),
        "explosion" => println!("🔊 💥 BOOOOM! 💥"),
        "sheer_heart_attack" => println!("🔊 *Mechanical whirring* SHEER HEART ATTACK!"),
        "bites_the_dust" => println!("🔊 ⏰ *Time reversal sound* BITES THE DUST!"),
        "menacing" => println!("🔊 ゴゴゴゴ (Menacing aura)"),
        _ => println!("🔊 *Unknown sound effect*"),
    }
}

/// Utility functions for battle calculations
pub mod battle_utils {
    use super::*;

    pub fn calculate_explosion_damage(base_power: u32, distance: f32) -> u32 {
        let damage_falloff = 1.0 - (distance / 10.0).min(1.0);
        (base_power as f32 * damage_falloff) as u32
    }

    pub fn get_stand_cry() -> &'static str {
        "KILLER QUEEN!"
    }

    pub fn format_jojo_text(text: &str) -> String {
        format!("✨ {} ✨", text.to_uppercase())
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_timed_bomb_creation() {
        let bomb = TimedBomb::new(BombType::Primary, 1, 100)
            .with_timer(std::time::Duration::from_secs(5));
        
        assert!(bomb.auto_detonate);
        assert!(bomb.timer_duration.is_some());
    }

    #[test]
    fn test_stand_stats() {
        let stats = StandStats::default();
        assert_eq!(stats.destructive_power, 5); // A rank
        assert_eq!(stats.range, 2); // D rank (close-range)
    }
}