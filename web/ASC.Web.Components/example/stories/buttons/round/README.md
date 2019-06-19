# DropDown

## Usage

```js
import { RoundButton } from 'asc-web-components';
```

#### Description

Round button with dropdown list

#### Usage

```js
<RoundButton
    data={[{key:"key",text:"text"}]}>
</RoundButton>
```

#### Properties

| Props              | Type     | Required | Values                      | Default        | Description                                                       |
| ------------------ | -------- | :------: | --------------------------- | -------------- | ----------------------------------------------------------------- |
| `title`           | `string`   |    -     | -                           | -        | Title text  
| `opened`           | `bool`   |    -     | -                           | `false`        | Tells when the dropdown should be opened  
| `data`           | `array`   |    -     | -                           | -        | Dropdown items array                          |