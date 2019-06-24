# DropDownItem

## Usage

```js
import { DropDownItem } from 'asc-web-components';
```

#### Description

Is a item of DropDown component

#### Usage

```js
<DropDownItem isSeparator={false} label='Button 1' onClick={() => console.log('Button 1 clicked')} />
```

#### Properties

| Props              | Type     | Required | Values                      | Default        | Description                                                       |
| ------------------ | -------- | :------: | --------------------------- | -------------- | ----------------------------------------------------------------- |
| `isSeparator`      | `bool`   |    -     | -                           | `false`        | Tells when the dropdown item should display like separator        |
| `label`            | `string` |    -     | -                           | `Dropdown item`| Dropdown item text                                                |
| `onClick`          | `func`   |    -     | -                           | -              | What the dropdown item will trigger when clicked                  |