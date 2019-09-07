# DropDownItem

## Usage

```js
import { DropDownItem } from 'asc-web-components';
```

#### Description

Is a item of DropDown component

#### Usage

```js
<DropDownItem 
    isSeparator={false}
    isHeader={false} 
    label='Button 1' 
    icon='NavLogoIcon' 
    onClick={() => console.log('Button 1 clicked')} />
```

#### Properties

| Props              | Type     | Required | Values                      | Default        | Description                                                       |
| ------------------ | -------- | :------: | --------------------------- | -------------- | ----------------------------------------------------------------- |
| `isSeparator`      | `bool`   |    -     | -                           | `false`        | Tells when the dropdown item should display like separator        |
| `isHeader`         | `bool`   |    -     | -                           | `false`        | Tells when the dropdown item should display like header           |
| `label`            | `string` |    -     | -                           | `Dropdown item`| Dropdown item text                                                |
| `icon`             | `string` |    -     | -                           | -              | Dropdown item icon                                                |
| `onClick`          | `func`   |    -     | -                           | -              | What the dropdown item will trigger when clicked                  |
| `disabled`         | `bool`   |    -     | -                           | `false`        | Tells when the dropdown item should display like disabled         |