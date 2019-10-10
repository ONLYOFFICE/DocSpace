# EmailInput

#### Description

Email entry field with advanced capabilities for displaying, validation of correspondence and generation based on settings.

#### Usage

```js
import { EmailInput } from "asc-web-components";

onChangeFunc = (value) => {
  // our code for working with input value
}

<EmailInput
  name="email"
  placeholder
  value={value}
  onChange={e => {
    this.onChangeFunc(e.target.value);
  }}
  isDisabled={false}
  placeholder="email"
  onValidateInput={a => console.log(a)}
/>;
```

#### Properties
You can apply all properties of the TextInput component to the component, except `mask` and `hasError` prop

| Props             | Type     | Required | Values             | Default         | Description                                                           |
| ----------------- | -------- | :------: | ------------------ | --------------- | --------------------------------------------------------------------- |
| `onValidateInput` | `func`   |    -     | -                  | -               | Will be validate our value, return Object, containing validating value and boolean validation result  |
