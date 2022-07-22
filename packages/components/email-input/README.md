# EmailInput

Email entry field with advanced capabilities for validation based on settings

### Usage

```js
import EmailInput from "@docspace/components/email-input";
import { EmailSettings } from "@docspace/components/utils/email";

const settings = new EmailSettings();

settings.allowDomainPunycode = true;
```

```jsx
<EmailInput
  name="email"
  placeholder="email"
  emailSettings={settings}
  onValidateInput={result =>
    console.log("onValidateInput", result.value, result.isValid, result.errors);
  }
/>;
```

### Properties

You can apply all properties of the `TextInput` component to the component

| Props             |           Type            | Required |             Values              |                                                                                      Default                                                                                       | Description                                                                                                                                                                                                          |
| ----------------- | :-----------------------: | :------: | :-----------------------------: | :--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------: | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `className`       |         `string`          |    -     |                -                |                                                                                         -                                                                                          | Accepts class                                                                                                                                                                                                        |
| `customValidate`  |          `func`           |    -     |                -                |                                                                                         -                                                                                          | Function for your custom validation input value. Function must return object with following parameters: `value`: string value of input, `isValid`: boolean result of validating, `errors`(optional): array of errors |
| `emailSettings`   | `Object`, `EmailSettings` |    -     |                -                | { allowDomainPunycode: false, allowLocalPartPunycode: false, allowDomainIp: false, allowStrictLocalPart: true, allowSpaces: false, allowName: false, allowLocalDomainName: false } | Settings for validating email                                                                                                                                                                                        |
| `hasError`        |          `bool`           |    -     |                -                |                                                                                         -                                                                                          | Used in your custom validation                                                                                                                                                                                       |
| `id`              |         `string`          |    -     |                -                |                                                                                         -                                                                                          | Accepts id                                                                                                                                                                                                           |
| `onChange`        |          `func`           |    -     |                -                |                                                                                         -                                                                                          | Function for your custom handling changes in input                                                                                                                                                                   |
| `onValidateInput` |          `func`           |    -     | { isValid: bool, errors: array} |                                                                                         -                                                                                          | Will be validate our value, return object with following parameters: `isValid`: boolean result of validating, `errors`: array of errors                                                                              |
| `style`           |      `obj`, `array`       |    -     |                -                |                                                                                         -                                                                                          | Accepts css style                                                                                                                                                                                                    |

### Validate email

Our validation algorithm based on [RFC 5322 email address parser](https://www.npmjs.com/package/email-addresses).

For email validating you should use plain Object or EmailSettings with following settings:

| Props                    |  Type  | Required | Default | Description                                                                                                                                                 |
| ------------------------ | :----: | :------: | :-----: | ----------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `allowDomainIp`          | `bool` |    -     | `false` | Allow email with IP address in domain, e. g. `user@[127.0.0.1]` is a valid email address                                                                    |
| `allowDomainPunycode`    | `bool` |    -     | `false` | Allow email with punycode symbols in domain, e. g. `example@джpумлатест.bрфa` and `example@mañana.com` are valid email addresses                            |
| `allowLocalDomainName`   | `bool` |    -     | `false` | Allow local domain address, e. g. `admin@local` is a valid email address                                                                                    |
| `allowLocalPartPunycode` | `bool` |    -     | `false` | Allow email with punycode symbols in local part, e. g. `джумла@example.com` and `mañana@example.com` are valid email addresses                              |
| `allowName`              | `bool` |    -     | `false` | Supports all features of RFC 5322, which means that `"Bob Example" <bob@example.com>` is a valid email address                                              |
| `allowSpaces`            | `bool` |    -     | `false` | Allow spaces in local part and domain, e. g. `" "@example.org` is a valid email address                                                                     |
| `allowStrictLocalPart`   | `bool` |    -     | `true`  | Allow email, started with latin symbols and digits(`([a-zA-Z0-9]+)`) and also contains `_,-,.,+`. Used RegEx `/^([a-zA-Z0-9]+)([_\-\.\+][a-zA-Z0-9]+)\*\$/` |

### emailSettings prop

Plain object:

```js
const emailSettings = {
  allowDomainPunycode: false,
  allowLocalPartPunycode: false,
  allowDomainIp: false,
  allowStrictLocalPart: true,
  allowSpaces: false,
  allowName: false,
  allowLocalDomainName: false,
};
```

or instance of `EmailSettings` class:

```js
import { EmailInput, utils } from "@docspace/components";
const { EmailSettings } = utils.email;

const emailSettings = new EmailSettings();

emailSettings.toObject(); /* returned Object with default settings:
{
  allowDomainPunycode: false,
  allowLocalPartPunycode: false,
  allowDomainIp: false,
  allowStrictLocalPart: true,
  allowSpaces: false,
  allowName: false,
  allowLocalDomainName: false
}
*/
email.allowName = true; // set allowName setting to true

emailSettings.toObject(); /* returned Object with NEW settings:
{
  
  allowDomainPunycode: false,
  allowLocalPartPunycode: false,
  allowDomainIp: false,
  allowStrictLocalPart: true,
  allowSpaces: false,
  allowName: true,
  allowLocalDomainName: false

}
*/
```

### Custom validate email

You should use custom validation with the `customValidate` prop. This prop contains function for your custom validation input value. Function must return object with following parameters: `value`: string value of input, `isValid`: boolean result of validating, `errors`(optional): array of errors.

Base colors:

| Сomponent actions | isValid |                           border-color                           |
| ----------------- | :-----: | :--------------------------------------------------------------: |
| `:focus`          | `false` |     ![#c30](https://placehold.it/15/c30/000000?text=+) #c30      |
| `:focus`          | `true`  | ![#2DA7DB](https://placehold.it/15/2DA7DB/000000?text=+) #2DA7DB |
| `:hover`          | `false` |     ![#c30](https://placehold.it/15/c30/000000?text=+) #c30      |
| `:hover`          | `true`  | ![#D0D5DA](https://placehold.it/15/D0D5DA/000000?text=+) #D0D5DA |
| `default`         | `false` |     ![#c30](https://placehold.it/15/c30/000000?text=+) #c30      |
| `default`         | `true`  | ![#D0D5DA](https://placehold.it/15/D0D5DA/000000?text=+) #D0D5DA |

```js
import React from "react";
import { EmailInput } from "@docspace/components";

const onChange = (e) => {
  // your event handling
  customValidate(e.target.value);
};

const customValidate = (value) => {
  const isValid = !!(value && value.length > 0);
  return {
    value,
    isValid: isValid,
    errors: isValid ? [] : ["incorrect email"],
  };
};

const onValidateInput = (result) => {
  console.log("onValidateInput", result);
};
```

```jsx
<EmailInput
  customValidate={customValidate}
  onChange={onChange}
  onValidateInput={onValidateInput}
/>
```
