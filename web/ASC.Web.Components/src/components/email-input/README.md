# EmailInput

Email entry field with advanced capabilities for validation based on settings

### Usage

```js
import { EmailInput } from "asc-web-components";
```

```jsx
<EmailInput
  name="email"
  placeholder="email"
  onValidateInput={isValidEmail =>
    console.log("isValidEmail = ", isValidEmail);
  }
  emailSettings={settings}
/>;
```

### Properties

You can apply all properties of the `TextInput` component to the component

| Props                |                 Type                  | Required | Values |     Default     | Description                                                                                                                                                                          |
| -------------------- | :-----------------------------------: | :------: | :----: | :-------------: | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `className`          |               `string`                |    -     |   -    |        -        | Accepts class                                                                                                                                                                        |
| `customValidateFunc` |                `func`                 |    -     |   -    |        -        | Function for your custom validation input value. Function must return object with following parameters: `isValid`: boolean result of validating, `errors`(optional): array of errors |
| `emailSettings`      | `Object`, `Instance of EmailSettings` |    -     |   -    | `EmailSettings` | Settings for validating email                                                                                                                                                        |
| `id`                 |               `string`                |    -     |   -    |        -        | Accepts id                                                                                                                                                                           |
| `isValid`            |                `bool`                 |    -     |   -    |        -        | Used in your custom validation function for change border-color of input                                                                                                             |
| `onChange`           |                `func`                 |    -     |   -    |        -        | Function for your custom handling changes in input                                                                                                                                   |
| `onValidateInput`    |                `func`                 |    -     |  { isValid: bool, errors: array}    |        -        | Will be validate our value, return object with following parameters: `isValid`: boolean result of validating, `errors`: array of errors                                               |
| `style`              |            `obj`, `array`             |    -     |   -    |        -        | Accepts css style                                                                                                                                                                    |

### Validate email

Our validation algorithm based on [An RFC 5322 email address parser](https://www.npmjs.com/package/email-addresses).

For email validating you should use plain Object or our email utility with following settings:

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
  allowLocalDomainName: false
};
```

or instance of `EmailSettings` class:

```js
import { EmailInput, utils } from "asc-web-components";
const { EmailSettings } = utils.email;

const emailSettings = new EmailSettings();

emailSettings.getSettings(); /* returned Object with default settings:
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

emailSettings.getSettings(); /* returned Object with NEW settings:
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

You should use custom validation with the `customValidateFunc` prop. This prop contain function for your custom validation input value. Function must return object with following parameters: `isValid`: boolean result of validating, `errors`(optional): array of errors.

How are applied colors in component:

| Сomponent actions | isValid | border-color |
| ----------------- | :-----: | :----------: |
| `:focus`          | `false` |     #c30     |
| `:focus`          | `true`  |   #2DA7DB    |
| `:hover`          | `false` |     #c30     |
| `:hover`          | `true`  |   #D0D5DA    |
| `default`         | `false` |     #c30     |
| `default`         | `true`  |   #D0D5DA    |

```js
import React from "react";
import { EmailInput } from "asc-web-components";


const customChangeFunc = (e) => {
  // your event handling
  customValidateFunc(e.target.value);
}

const customValidateFunc = (value) => {
  let validationResult;
// your validating function
const emailValidation = {
  isValid: true,
  errors: ['Too short username', 'Incorrect domain']
}
  return emailValidation;
}

const onValidateInput = (isValidEmailObj) => {
    console.log(`isValidEmail = ${isValidEmailObj.isValid}`);
}

return (
<EmailInput
  onChange={customChangeFunc}
  customValidateFunc={customValidateFunc}
  onValidateInput={onValidateInput}

/>;
);
```

#### Email settings RFC 5321

```js
{
  allowDomainPunycode: true,
  allowLocalPartPunycode: true,
  allowDomainIp: true,
  allowStrictLocalPart: false,
  allowSpaces: true,
  allowName: false,
  allowLocalDomainName: true
}
```
