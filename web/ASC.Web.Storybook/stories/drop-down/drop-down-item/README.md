# DropDownItem

## Usage

```js
import { DropDownItem } from 'asc-web-components';
```

#### Description

Is a item of DropDown component

To add an avatar username and email when you turn on the isUserPreview parameter, you need to add the parameters of the Avatar component: source - link to the user's avatar, userName - user name and label - user’s email address.

#### Usage

```js
<DropDownItem isSeparator={false} isUserPreview={false} label='Button 1' onClick={() => console.log('Button 1 clicked')} />
```

#### Properties

| Props              | Type     | Required | Values                      | Default        | Description                                                       |
| ------------------ | -------- | :------: | --------------------------- | -------------- | ----------------------------------------------------------------- |
| `isSeparator`      | `bool`   |    -     | -                           | `false`        | Tells when the dropdown item should display like separator        |
| `isUserPreview`    | `bool`   |    -     | -                           | `false`        | Tells when the dropdown item should display like User preview     |
| `isHeader`         | `bool`   |    -     | -                           | `false`        | Tells when the dropdown item should display like header           |
| `label`            | `string` |    -     | -                           | `Dropdown item`| Dropdown item text                                                |
| `onClick`          | `func`   |    -     | -                           | -              | What the dropdown item will trigger when clicked                  |
| `disabled          | `bool`   |    -     | -                           | `false`        | Tells when the dropdown item should display like disabled         |