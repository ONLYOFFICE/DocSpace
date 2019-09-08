# ToggleContent

## Usage

```js
import { ToggleContent } from 'asc-web-components';
```

#### Description

ToggleContent allow you to adding information, which you may hide/show by clicking  header

#### Usage

```js
    <ToggleContent>
        <span>Some text</span>
    </ToggleContent>
```

#### Properties

| Props              | Type     | Required | Values                      | Default        | Description                                                       |
| ------------------ | -------- | :------: | --------------------------- | -------------- | ----------------------------------------------------------------- |
| `label`             | `text`  |    ✅    | - | Some label     | Define label for header                                        |
| `isOpen`             | `bool`  |   -     | - | `false`     | State of component                                        |