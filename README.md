# ProjectBeta

## Demo
#### Rotating on slopes
![Rotating on slopes](https://user-images.githubusercontent.com/19392641/234477724-1b056421-c73f-4f69-bdfa-1337f7b4578a.gif)
#### Jumping
![Movie_005](https://user-images.githubusercontent.com/19392641/234477823-0a8727d2-9043-48b9-b564-f2505fbbe225.gif)
#### Climbing the stars
![Movie_004](https://user-images.githubusercontent.com/19392641/234477808-6ab34e1c-695f-4c6c-bce4-a13f51688dae.gif)



## Architecture
The `Player` reads the input and passes it to the [CharacterController](https://github.com/YegorStepanov/ProjectBeta/blob/master/Assets/Scripts/Character/CustomCharacterController.cs).
`CharacterController` implements the `ICharacterController` interface. When these methods are called, depending on the current state, the **features** are called in the correct and explicit order.
#### Important Notes
- `ICharacterController` is an interfrace from the low-level [Kinematic Character Controller](https://assetstore.unity.com/packages/tools/physics/kinematic-character-controller-99131) package. Its engine calls the implemented methods (`UpdateRotation`, `BeforeCharacterUpdate`, etc) when needed.  
- **Feature** is an encapsulated logic of `CharacterController`: `MovementFeature`, `JumpingFeature` etc.  

#### Example
```C#
public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
{
    switch (CurrentState)
    {
        case CharacterState.Default:
            _movementFeature.UpdateVelocity(ref currentVelocity, deltaTime);
            _jumpingFeature.UpdateVelocity(ref currentVelocity, deltaTime);
            _gravityFeature.UpdateVelocity(ref currentVelocity, deltaTime);
            _dragFeature.UpdateVelocity(ref currentVelocity, deltaTime);
            _externalForceFeature.UpdateVelocity(ref currentVelocity, deltaTime);
            break;
        case CharacterState.Charging:
            _chargingFeature.UpdateVelocity(ref currentVelocity, deltaTime);
            break;
        case CharacterState.NoClip:
            _noClipFeature.UpdateVelocity(ref currentVelocity, deltaTime);
            break;
        default:
            break;
    }
}
```

## Settings
![image](https://user-images.githubusercontent.com/19392641/234478640-fb7964bf-3472-44d4-a878-f30fd02d6bb8.png)

![image](https://user-images.githubusercontent.com/19392641/234478650-87337e1a-63de-4400-8b6c-d9ddd40dc84d.png)
