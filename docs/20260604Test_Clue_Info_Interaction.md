# Development Log - June 4, 2026

## Testing Progress Summary

### Issue 1: Nothing triggered initially
- **Symptom**: No interaction or trigger detection when approaching objects
- **Cause**: Player missing Capsule Collider component
- **Fix**: Added Capsule Collider to PlayerCapsule

### Issue 2: Player flew to the ceiling when jumping
- **Symptom**: After adding Capsule Collider, jumping caused the player to launch upward uncontrollably
- **Cause**: Collider physics conflict (CharacterController + Capsule Collider interference)
- **Fix**: Removed the extra Capsule Collider — CharacterController handles collision internally

### Issue 3: Notepad couldn't detect player proximity
- **Symptom**: `OnTriggerEnter` never triggered when approaching Notepad
- **Cause**: Trigger Collider size was too flat/insufficient to reach player's collision body
- **Fix**: Increased the Trigger Collider size and raised its Center Y position to cover the player's height range

### Issue 4: Empty GameObject ViewPoint didn't work for camera switching
- **Symptom**: Camera didn't transition to ViewPoint when pressing E
- **Cause**: Manual camera movement with Transform caused conflicts and unpredictable behavior
- **Fix**: Switched to **Cinemachine Virtual Camera** for smooth, reliable camera switching

---

## Current Status

- ✅ Player movement and jumping are normal
- ✅ Notepad triggers `OnTriggerEnter` when player approaches
- ✅ Cinemachine Virtual Camera is set up for viewing interactions
- 🔄 Fine-tuning Virtual Camera position and rotation for optimal viewing angle

## Next Steps

- Adjust VCam_Notepad position/rotation to ensure it looks at Notepad correctly
- Test UI panel display with proper text content
- Apply the working interaction system to other readable objects (screens, posters, code prints)