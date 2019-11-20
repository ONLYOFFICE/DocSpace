# ComboBox

Custom combo box input

### Usage

```js
import { ComboBox } from "asc-web-components";
```

```js
const options = [
  {
    key: 1,
    icon: "CatalogEmployeeIcon", // optional item
    label: "Option 1",
    disabled: false, // optional item
    onClick: clickFunction // optional item
  }
];
```

```jsx
<ComboBox
  options={options}
  isDisabled={false}
  selectedOption={{
    key: 0,
    label: "Select"
  }}
  dropDownMaxHeight={200}
  noBorder={false}
  scale={true}
  scaledOptions={true}
  size="content"
  onSelect={option => console.log("selected", option)}
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
  onSelect={option => console.log("Selected option", option)}
  selectedOption={{
    key: 0,
    label: "Select"
  }}
  isDisabled={false}
  scaled={false}
  size="content"
  directionX="right"
>
  <Icons.NavLogoIcon size="medium" key="comboIcon" />
</ComboBox>
```

### Properties

| Props               |   Type    | Required |                   Values                   | Default | Description                                                 |
| ------------------- | :-------: | :------: | :----------------------------------------: | :-----: | ----------------------------------------------------------- |
| `options`           |  `array`  |    ✅    |                     -                      |    -    | Combo box options                                           |
| `isDisabled`        |  `bool`   |    -     |                     -                      | `false` | Indicates that component is disabled                        |
| `noBorder`          |  `bool`   |    -     |                     -                      | `false` | Indicates that component is displayed without borders       |
| `selectedOption`    | `object`  |    ✅    |                     -                      |    -    | Selected option                                             |
| `onSelect`          |  `func`   |    -     |                     -                      |    -    | Will be triggered whenever an ComboBox is selected option   |
| `dropDownMaxHeight` | `number`  |    -     |                     -                      |    -    | Height of Dropdown                                          |
| `scaled`            |  `bool`   |    -     |                     -                      | `true`  | Indicates that component is scaled by parent                |
| `scaledOptions`     |  `bool`   |    -     |                     -                      | `false` | Indicates that component`s options is scaled by ComboButton |
| `size`              |  `oneOf`  |    -     | `base`, `middle`, `big`, `huge`, `content` | `base`  | Select component width, one of default                      |
| `advancedOptions`   | `element` |    -     |                     -                      |    -    | If you need display options not basic options               |

## ComboButton

#### Description

> This description is for reference only, the component described below is not exported.

To create designs using combobox logic, there is a child component ComboButton.
This is an independent element that responds to changes in parameters and serves only to demonstrate set values.

```jsx
<ComboButton
  noBorder={false}
  isDisabled={false}
  selectedOption={{
    key: 0,
    label: "Select"
  }}
  withOptions={false}
  optionsLength={0}
  withAdvancedOptions={true}
  innerContainer={<>Demo container</>}
  innerContainerClassName="optionalBlock"
  isOpen={false}
  size="content"
  scaled={false}
/>
```

### Properties

| Props                     |   Type   | Required |              Values              |     Default      | Description                                              |
| ------------------------- | :------: | :------: | :------------------------------: | :--------------: | -------------------------------------------------------- |
| `isDisabled`              |  `bool`  |    -     |                -                 |     `false`      | Indicates that component is disabled                     |
| `noBorder`                |  `bool`  |    -     |                -                 |     `false`      | Indicates that component is displayed without borders    |
| `selectedOption`          | `object` |    -     |                -                 |        -         | Selected option                                          |
| `withOptions`             |  `bool`  |    -     |                -                 |      `true`      | Lets you style as ComboBox with options                  |
| `optionsLength`           | `number` |    -     |                -                 |        -         | Lets you style as ComboBox with options                  |
| `withAdvancedOptions`     |  `bool`  |    -     |                -                 |     `false`      | Lets you style as a ComboBox with advanced options       |
| `innerContainer`          |  `node`  |    -     |                -                 |        -         | Allows displaying third-party element inside ComboButton |
| `innerContainerClassName` | `string` |    -     |                -                 | `innerContainer` | Required to access third-party container                 |
| `isOpen`                  |  `bool`  |    -     |                -                 |     `false`      | Lets you style as ComboBox arrow                         |
| `scaled`                  |  `bool`  |    -     |                -                 |     `false`      | Indicates that component is scaled by parent             |
| `size`                    | `oneOf`  |    -     | `base`, `...`, `huge`, `content` |    `content`     | Select component width, one of default                   |
| `onClick`                 |  `func`  |    -     |                -                 |        -         | Will be triggered whenever an ComboButton is clicked     |
