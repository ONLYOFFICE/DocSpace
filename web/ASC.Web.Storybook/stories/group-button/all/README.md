# GroupButton

## Usage

```js
import { GroupButton } from 'asc-web-components';
```

#### Description

Base Button is used for a group action on a page.

#### Usage

```js
<GroupButton text='Group button' primary={false} disabled={false} isCheckbox={false} isDropdown={false} splitted={false} opened={false} ></GroupButton>
```

#### Properties

| Props              | Type     | Required | Values                      | Default        | Description                                                       |
| ------------------ | -------- | :------: | --------------------------- | -------------- | ----------------------------------------------------------------- |
| `text`             | `string` |    -     | -                           | `Default text` | Value of the group button                                         |
| `primary`          | `bool`   |    -     | -                           | `false`        | Tells when the button should be primary                           |
| `disabled`         | `bool`   |    -     | -                           | `false`        | Tells when the button should present a disabled state             |
| `isCheckbox`       | `bool`   |    -     | -                           | `false`        | Tells when the button should present a checkbox state             |
| `isDropdown`       | `bool`   |    -     | -                           | `false`        | Tells when the button should present a dropdown state             |
| `splitted`         | `bool`   |    -     | -                           | `false`        | Tells when the button should present a dropdown state with button |
| `opened`           | `bool`   |    -     | -                           | `false`        | Tells when the button should be opened by default                 |