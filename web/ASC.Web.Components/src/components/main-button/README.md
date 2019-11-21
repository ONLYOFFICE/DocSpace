# MainButton

The main button is located at the top of the main menu. It may consist of 2 buttons, or on a click to show a pop-up window with other buttons

### Usage

```js
import { MainButton } from "asc-web-components";
```

```jsx
<MainButton text="Button" isDisabled={false} isDropdown={true}>
  <div>Some button</div>
  <Button label="Some button" />
</MainButton>
```

or

```jsx
<MainButton
  text="Button"
  isDropdown={false}
  clickAction={() => SomeFunction()}
  clickActionSecondary={() => SomeFunction()}
  moduleName="people"
/>
```

#### Properties

| Props                  |   Type   | Required |            Values             |   Default    | Description                                                              |
| ---------------------- | :------: | :------: | :---------------------------: | :----------: | ------------------------------------------------------------------------ |
| `isDisabled`           |  `bool`  |    -     |               -               |   `false`    | Tells when the button should present a disabled state                    |
| `isDropdown`           |  `bool`  |    -     |               -               |    `true`    | Select a state between two separate buttons or one with a drop-down list |
| `clickAction`          |  `func`  |    -     |               -               |      -       | What the main button will trigger when clicked                           |
| `clickActionSecondary` |  `func`  |    -     |               -               |      -       | What the secondary button will trigger when clicked                      |
| `moduleName`           | `oneOf`  |    -     | `people`, `mail`, `documents` |      -       | The name of the module where the button is used                          |
| `text`                 | `string` |    -     |               -               |   `Button`   | Button text                                                              |
| `iconName`             | `string` |    -     |               -               | `PeopleIcon` | Icon inside button                                                       |
