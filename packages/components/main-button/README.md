# MainButton

The main button is located at the top of the main menu. It may consist of 2 buttons, or on a click to show a pop-up window with other buttons

### Usage

```js
import MainButton from "@docspace/components/main-button";
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
  onkAction={() => SomeFunction()}
  moduleName="people"
/>
```

#### Properties

| Props        |      Type      | Required |            Values             |   Default    | Description                                                              |
| ------------ | :------------: | :------: | :---------------------------: | :----------: | ------------------------------------------------------------------------ |
| `className`  |    `string`    |    -     |               -               |      -       | Accepts class                                                            |
| `onAction`   |     `func`     |    -     |               -               |      -       | What the main button will trigger when clicked                           |
| `iconName`   |    `string`    |    -     |               -               | `PeopleIcon` | Icon inside button                                                       |
| `id`         |    `string`    |    -     |               -               |      -       | Accepts id                                                               |
| `isDisabled` |     `bool`     |    -     |               -               |   `false`    | Tells when the button should present a disabled state                    |
| `isDropdown` |     `bool`     |    -     |               -               |    `true`    | Select a state between two separate buttons or one with a drop-down list |
| `moduleName` |    `oneOf`     |    -     | `people`, `mail`, `documents` |      -       | The name of the module where the button is used                          |
| `onClick`    |     `func`     |    -     |               -               |      -       | DropDown component click action                                          |
| `opened`     |     `bool`     |    -     |               -               |      -       | Open DropDown                                                            |
| `style`      | `obj`, `array` |    -     |               -               |      -       | Accepts css style                                                        |
| `text`       |    `string`    |    -     |               -               |   `Button`   | Button text                                                              |
