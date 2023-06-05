# ComboBox

Custom combo box input

### Usage

```js
import ComboBox from "@docspace/components/combobox";
import NavLogoReactSvg from "PUBLIC_DIR/images/nav.logo.react.svg";
import CatalogEmployeeReactSvg from "PUBLIC_DIR/images/catalog.employee.react.svg?url";
```

```js
const options = [
  {
    key: 1,
    icon: CatalogEmployeeReactSvg, // optional item
    label: "Option 1",
    disabled: false, // optional item
    onClick: clickFunction, // optional item
  },
];
```

```jsx
<ComboBox
  options={options}
  isDisabled={false}
  selectedOption={{
    key: 0,
    label: "Select",
  }}
  dropDownMaxHeight={200}
  noBorder={false}
  scale={true}
  scaledOptions={true}
  size="content"
  onSelect={(option) => console.log("selected", option)}
/>
```

Options have options:

- key - Item key, may be a string or a number
- label - Display text
- icon - Optional name of icon that will be displayed before label
- disabled - Make option disabled
- onClick - On click function

ComboBox perceives all property`s for positioning from DropDown!

If you need to display a custom list of options, you must use advancedOptions property. Like this:

```js
const advancedOptions = (
  <>
    <DropDownItem>
      <RadioButton value="asc" name="first" label="A-Z" isChecked={true} />
    </DropDownItem>
    <DropDownItem>
      <RadioButton value="desc" name="first" label="Z-A" />
    </DropDownItem>
    <DropDownItem isSeparator />
    <DropDownItem>
      <RadioButton value="first" name="second" label="First name" />
    </DropDownItem>
    <DropDownItem>
      <RadioButton
        value="last"
        name="second"
        label="Last name"
        isChecked={true}
      />
    </DropDownItem>
  </>
);
```

```jsx
<ComboBox
  options={[]} // An empty array will enable advancedOptions
  advancedOptions={advancedOptions}
  onSelect={(option) => console.log("Selected option", option)}
  selectedOption={{
    key: 0,
    label: "Select",
  }}
  isDisabled={false}
  scaled={false}
  size="content"
  directionX="right"
>
  <NavLogoReactSvg size="medium" key="comboIcon" />
</ComboBox>
```

To use Combobox as a toggle button, you must declare it according to the parameters:

```jsx
<ComboBox
  options={[]} // Required to display correctly
  selectedOption={{
    key: 0,
    label: "Selected option",
  }}
  scaled={false}
  size="content"
  displayType="toggle"
  onToggle={alert("action")}
>
  <NavLogoReactSvg size="medium" key="comboIcon" />
</ComboBox>
```

### Properties

| Props               |      Type      | Required |                   Values                   |  Default  | Description                                                                            |
| ------------------- | :------------: | :------: | :----------------------------------------: | :-------: | -------------------------------------------------------------------------------------- |
| `advancedOptions`   |   `element`    |    -     |                     -                      |     -     | If you need display options not basic options                                          |
| `className`         |    `string`    |    -     |                     -                      |     -     | Accepts class                                                                          |
| `displayType`       |    `oneOf`     |    -     |            `default`, `toggle`             | `default` | Component Display Type                                                                 |
| `dropDownMaxHeight` |    `number`    |    -     |                     -                      |     -     | Height of Dropdown                                                                     |
| `id`                |    `string`    |    -     |                     -                      |     -     | Accepts id                                                                             |
| `isDisabled`        |     `bool`     |    -     |                     -                      |  `false`  | Indicates that component is disabled                                                   |
| `noBorder`          |     `bool`     |    -     |                     -                      |  `false`  | Indicates that component is displayed without borders                                  |
| `onSelect`          |     `func`     |    -     |                     -                      |     -     | Will be triggered whenever an ComboBox is selected option                              |
| `options`           |    `array`     |    ✅    |                     -                      |     -     | Combo box options                                                                      |
| `scaledOptions`     |     `bool`     |    -     |                     -                      |  `false`  | Indicates that component`s options is scaled by ComboButton                            |
| `scaled`            |     `bool`     |    -     |                     -                      |  `true`   | Indicates that component is scaled by parent                                           |
| `selectedOption`    |    `object`    |    ✅    |                     -                      |     -     | Selected option                                                                        |
| `size`              |    `oneOf`     |    -     | `base`, `middle`, `big`, `huge`, `content` |  `base`   | Select component width, one of default                                                 |
| `style`             | `obj`, `array` |    -     |                     -                      |     -     | Accepts css style                                                                      |
| `onToggle`          |     `func`     |    -     |                     -                      |     -     | The event will be raised when using `displayType: toggle` when clicking on a component |
| `showDisabledItems` |     `bool`     |    -     |                     -                      |  `false`  | Display disabled items or not when displayType !== toggle                              |
