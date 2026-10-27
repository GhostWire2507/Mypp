For your Unity project, I would replace the current free-flying camera with a snappy third-person follow camera attached to the playable character. The camera should follow the character around the multimedia lab environment, including the stairs and balcony, rather than letting the player fly around the scene independently.

The important distinction is that the camera should follow the character, not control the character's movement.

## How the camera should behave

[Creatures of Ava - Gamereactor 中文版](https://images.openai.com/static-rsc-4/J24Itaqfy6obKTLXKL12DaeK8hZLt6GkW0t0xfz5uX6d0c6hCX3iIWH7BiueSAihebyH4YHndCBF9nV40WOXyIGW2GyH4CCL9AbfWnpqpP7WmDa5C1dIvKWHNMUzkFBek1j2O0UKSmcg28sakdmfMJ514j3vyMhc0zg6cPT9SCY?purpose=inline)

1\. Character-following camera

The camera stays behind and slightly above the character. When the character moves, the camera follows at a consistent distance.

[Mafia: The Old Country - PC performance graphics benchmarks of Graphics Cards and Processors](https://images.openai.com/static-rsc-4/j6ja4ycOgpShuHYlbGA96IEJCCCGauEOTOR3XXoR_oTWgdpglsqultcBOL95Ss_5Lf7wqDBAmlzzLKnNY5EuJJr9zD0EmM9anb2tlBSv1njU_6oSuHhLO_6raNOYr-zJYlACj-JEzIhmrmrL9hlfLhiMXEW2hW_SyRkb4YZI9SI?purpose=inline)

2\. Responsive mouse control

Moving the mouse rotates the camera around the character. It should respond quickly without feeling excessively loose or sluggish.

[Mafia 3: Smuggling (smuggling racket) - gamepressure.com](https://images.openai.com/static-rsc-4/aRG_oE2Zx2-nZZ_iA5VTgv_t45LNv7dbIk82Olid7IzwfeBNiIRgPZJc3BUhy1VKuphexq4VDtzRuSH0bamGt_NQef3pzdEMnzKTG5rF5wgbvcwKzgzognoMs2q83nru266dK2aWLBsZ0lIbsTCJNnAcLDB24CsA3uo42B-XG3c?purpose=inline)

3\. Proper stair and balcony viewing

The camera should follow the character up and down the staircase, keep the character visible, and avoid clipping through the building, walls, railings, or floor.

## Prompt for your Unity MCP

# Unity MCP Task: Replace the Free-Flying Camera with a Responsive Third-Person Character Camera

Modify my existing Unity project to use a proper third-person gameplay camera instead of the current completely free camera.

The project contains a building environment with multiple floors, exterior walkways, railings, stairs, and a balcony. I want to explore this environment as a character walking through it, with the camera following that character in a responsive third-person view.

Inspect the existing scene, scripts, player objects, and input configuration before making changes. Implement the working camera and character-control system directly in the project.

## 1. Main objective

Create a third-person exploration system with these characteristics:

- The player controls a character that walks around the environment.
- The camera follows behind and slightly above the character.
- Mouse movement rotates the camera around the character.
- Keyboard movement moves the character relative to the camera's horizontal facing direction.
- The camera responds quickly and feels controlled rather than floaty.
- The character can walk through doors and along corridors, ascend and descend stairs, and walk on the balcony.
- The camera should not clip excessively through walls, floors, railings, or other building geometry.

This is an architectural environment exploration experience, not a combat game. Prioritize reliable movement, camera responsiveness, and visibility of the building.

## 2. Third-person camera behaviour

Implement a camera that orbits around a designated follow target attached to the character.

**Starting camera settings:**

- Camera position: behind and above the character.
- Follow distance: approximately 3–4 metres.
- Look target: approximately 1.4–1.7 metres above the character's feet.
- Camera rotation: controlled by mouse movement.
- Horizontal rotation: full 360-degree orbit.
- Vertical rotation: limited so the camera cannot flip upside down or point excessively above or below the character.
- Follow smoothing: light and responsive, not slow or floaty.
- Camera rotation smoothing: minimal, with immediate-feeling mouse response.
- Camera collision: shorten the camera distance when a wall or other obstruction is between the camera and its target.

Treat these values as initial tuning targets, not mandatory dimensions. Adjust them to suit the character scale and building geometry.

### Camera responsiveness

I want a snappy camera.

- Mouse input must produce predictable, responsive camera rotation.
- Avoid excessive acceleration, inertia, delayed rotation, or spring-like movement.
- The camera should settle behind the character naturally when the character changes direction, if automatic recentering is implemented.
- Do not force the camera to recenter continuously while I am inspecting the building.
- Keep the character visible during normal movement.
- Avoid sudden camera jumps when the character moves between floors.

Use a straightforward, maintainable implementation. Use Cinemachine if it is already installed and suitable; otherwise, a custom camera-follow script is acceptable.

## 3. Character movement

Inspect the project first to determine whether a suitable character controller or animated character already exists.

If a suitable character exists, reuse it. If not, create a basic third-person player character using the existing available assets or a simple placeholder model.

Implement these controls:

- **W:** Move forward relative to the camera.
- **S:** Move backward.
- **A:** Move left.
- **D:** Move right.
- **Mouse:** Rotate the camera around the character.

Movement must be relative to the camera's horizontal direction. Pressing W should move the character in the direction the camera is facing across the ground, regardless of the camera's vertical viewing angle.

The character should rotate smoothly toward the direction of travel. Do not make the character rotate unpredictably when the camera moves.

Use Unity's existing input system if possible. Inspect whether the project uses the legacy Input Manager, the new Input System, or an existing input abstraction. Do not introduce incompatible input dependencies.

## 4. Walking on stairs and across the building

The environment includes stairs, elevated walkways, landings, doors, and a balcony.

The character controller must support this environment.

- Use a CharacterController or an appropriate existing movement system.
- Configure step offset, slope handling, gravity, and ground detection appropriately for the staircase dimensions.
- Keep the character grounded on stair treads and landings.
- Allow movement between the ground floor and upper-level balcony.
- Ensure the character does not fall through the floor or walk through solid walls.
- Check that the floor, stair treads, balcony, and landing have suitable colliders.
- Avoid adding colliders indiscriminately to decorative objects if they would obstruct movement.
- Do not teleport the character between levels to simulate stair movement.

If the existing stair geometry or colliders make traversal impossible, report the specific problem and make only the necessary corrections.

## 5. Camera collision and visibility

The camera must work inside and outside the building.

- Prevent the camera from passing through solid walls or the floor during normal gameplay.
- When approaching a wall, move the camera closer to the character temporarily.
- Restore the normal follow distance when the obstruction clears.
- Avoid placing the camera on the opposite side of the building from the character.
- Keep the character visible when ascending the stairs and walking underneath the balcony.
- Do not let railings cause constant camera snapping or unnecessary zooming.
- Avoid excessive camera jitter around stair edges and narrow corridors.

Use a suitable collision layer mask and verify the behaviour against the actual building geometry.

## 6. Input and gameplay usability

When entering Play Mode:

- The character should be positioned at a sensible starting point on the ground floor.
- The camera should immediately frame the character correctly.
- Mouse input should rotate the camera when gameplay has focus.
- The cursor should be locked and hidden during normal gameplay, with an appropriate way to release it using Escape.
- If the cursor is released, clicking back into the gameplay view should restore control.
- Movement should stop appropriately when the game loses focus.
- The camera should not depend on manually selecting and moving objects in the Unity Scene view.

Do not confuse the editor's free-flying Scene camera with the actual gameplay camera. The goal is to replace the free camera used by the game, not to interfere with Unity's Scene view navigation.

## 7. Unity implementation

Before editing:

1. Inspect the active scene and identify the current Main Camera and any camera-control scripts.
2. Identify existing character models, animations, movement scripts, colliders, and input dependencies.
3. Determine whether Cinemachine is installed.
4. Determine which GameObjects should be reused, configured, disabled, or created.

Then implement the system.

- Create or configure a player GameObject.
- Attach a suitable movement controller.
- Create a camera follow target at the appropriate height.
- Configure the gameplay camera to follow and orbit around that target.
- Add collision handling and camera-distance adjustment.
- Configure the necessary input bindings.
- Set the correct gameplay camera as the active Main Camera.
- Ensure there is only one active gameplay camera unless the project intentionally requires multiple cameras.
- Preserve the existing building, materials, lighting, imported assets, and unrelated scripts.
- Do not delete the existing free camera if it may still be useful for debugging; disable or replace its gameplay role safely.
- Ensure all required references are assigned and that no missing-script or null-reference errors are introduced.

Keep the implementation simple enough that I can later adjust camera sensitivity, follow distance, camera height, movement speed, and rotation responsiveness through the Unity Inspector.

## 8. Testing requirements

Enter Play Mode and test the actual implementation.

Verify that:

1. The camera follows the character when moving forward, backward, left, and right.
2. Mouse movement rotates the camera responsively.
3. Camera-relative movement works when the camera is rotated.
4. The character can climb and descend the staircase.
5. The character can walk along the balcony and through the intended building routes.
6. The camera does not consistently clip through the building or become stuck on railings.
7. The camera remains stable when the character changes elevation.
8. Escape releases the cursor and gameplay input can be restored.
9. No console errors prevent movement or camera operation.

If any test fails, investigate and correct the underlying issue rather than simply reporting that the scripts were created.

## Completion report

When finished, tell me:

- Which GameObjects and scripts were created or modified.
- Which camera system was used and why.
- The controls for playing the environment.
- The Inspector settings I can change to adjust camera behaviour.
- Whether the character can successfully traverse the stairs and balcony.
- Any remaining problems that require manual attention.

The final result must be a working third-person exploration system in Play Mode, not just a camera positioned behind a character in the editor.

## One important implementation detail

The camera alone is not enough. The MCP needs to establish three connected parts:

Input

WASD and mouse

Player controller

Movement, rotation, gravity and collisions

Third-person camera

Follow, orbit, smoothing and collision avoidance

My recommendation is to start with a fixed follow distance and responsive mouse orbit, then add camera collision handling and fine-tune the behaviour on the stairs. That is easier to debug than building an elaborate camera system immediately.