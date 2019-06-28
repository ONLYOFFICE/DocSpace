# DropDown

## Usage

```js
import { DropDown } from 'asc-web-components';
```

#### Description

Is a dropdown with any number of action

#### Usage

```js
<DropDown opened={false}></DropDown>
```

#### Properties

| Props              | Type     | Required | Values                      | Default        | Description                                                       |
| ------------------ | -------- | :------: | --------------------------- | -------------- | ----------------------------------------------------------------- |
| `opened`           | `bool`   |    -     | -                           | `false`        | Tells when the dropdown should be opened                          |
| `direction`        | `oneOf`  |    -     | `left`, `right`             | `left`         | Sets the opening direction relative to the parent                 |