import React, { useState } from "react";
import EmailInput from "./";

export default {
  title: "Components/EmailInput",
  component: EmailInput,
  argTypes: {
    allowDomainPunycode: { control: "boolean" },
    allowLocalPartPunycode: { control: "boolean" },
    allowDomainIp: { control: "boolean" },
    allowStrictLocalPart: { control: "boolean" },
    allowSpaces: { control: "boolean" },
    allowName: { control: "boolean" },
    allowLocalDomainName: { control: "boolean" },
    onValidateInput: { action: "onValidateInput" },
    onChange: { action: "onChange" },
  },
  parameters: {
    docs: {
      description: {
        component: `Email entry field with advanced capabilities for validation based on setting

### Properties

You can apply all properties of the 'TextInput' component to the component

| Props             |           Type            | Required |             Values              |                                                                                      Default                                                                                       | Description                                                                                                                                                                                                          |
| ----------------- | :-----------------------: | :------: | :-----------------------------: | :--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------: | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| className       |         string          |    -     |                -                |                                                                                         -                                                                                          | Accepts class                                                                                                                                                                                                        |
| customValidate  |          func           |    -     |                -                |                                                                                         -                                                                                          | Function for your custom validation input value. Function must return object with following parameters: 'value': string value of input, 'isValid': boolean result of validating, 'errors'(optional): array of errors |
| emailSettings   | Object, EmailSettings |    -     |                -                | { allowDomainPunycode: false, allowLocalPartPunycode: false, allowDomainIp: false, allowStrictLocalPart: true, allowSpaces: false, allowName: false, allowLocalDomainName: false } | Settings for validating email                                                                                                                                                                                        |
| hasError        |          bool           |    -     |                -                |                                                                                         -                                                                                          | Used in your custom validation                                                                                                                                                                                       |
| id              |         string          |    -     |                -                |                                                                                         -                                                                                          | Accepts id                                                                                                                                                                                                           |
| onChange        |          func           |    -     |                -                |                                                                                         -                                                                                          | Function for your custom handling changes in input                                                                                                                                                                   |
| onValidateInput |          func           |    -     | { isValid: bool, errors: array} |                                                                                         -                                                                                          | Will be validate our value, return object with following parameters: 'isValid': boolean result of validating, 'errors': array of errors                                                                              |
| style           |      obj, array       |    -     |                -                |                                                                                         -                                                                                          | Accepts css style     
        
### Validate email

Our validation algorithm based on [RFC 5322 email address parser](https://www.npmjs.com/package/email-addresses).

For email validating you should use plain Object or EmailSettings with following settings:

    const settings = {

      allowDomainPunycode,

      allowLocalPartPunycode,

      allowDomainIp,

      allowStrictLocalPart,

      allowSpaces,

      allowName,

      allowLocalDomainName,
    };

### emailSettings prop

Plain object:

    const emailSettings = {

      allowDomainPunycode: false,

      allowLocalPartPunycode: false,

      allowDomainIp: false,

      allowStrictLocalPart: true,

      allowSpaces: false,

      allowName: false,

      allowLocalDomainName: false,
    };

or instance of 'EmailSettings' class:

    import { EmailInput, utils } from "@appserver/components";

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

### Custom validate email

You should use custom validation with the 'customValidate' prop. This prop contains function for your custom validation input value. Function must return object with following parameters: 'value': string value of input, 'isValid': boolean result of validating, 'errors'(optional): array of errors.

Base colors:

| Сomponent actions | isValid |                           border-color                           |
| ----------------- | :-----: | :--------------------------------------------------------------: |
|   :focus          | false |     ![#c30](https://placehold.it/15/c30/000000?text=+) #c30      |
|   :focus          | true  | ![#2DA7DB](https://placehold.it/15/2DA7DB/000000?text=+) #2DA7DB |
|   :hover          | false |     ![#c30](https://placehold.it/15/c30/000000?text=+) #c30      |
|   :hover          | true  | ![#D0D5DA](https://placehold.it/15/D0D5DA/000000?text=+) #D0D5DA |
|   default         | false |     ![#c30](https://placehold.it/15/c30/000000?text=+) #c30      |
|   default         | true  | ![#D0D5DA](https://placehold.it/15/D0D5DA/000000?text=+) #D0D5DA |

    import React from "react";

    import { EmailInput } from "@appserver/components";


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


    <EmailInput

      customValidate={customValidate}

      onChange={onChange}

      onValidateInput={onValidateInput}
    />

          
          `,
      },
      source: {
        code: `import EmailInput from "@appserver/components/email-input";
import { EmailSettings } from "@appserver/components/utils/email";

const settings = new EmailSettings();
settings.allowDomainPunycode = true;

<EmailInput
  name="email"
  placeholder="email"
  emailSettings={settings}
  onValidateInput={result =>
    console.log("onValidateInput", result.value, result.isValid, result.errors);
  }
/>
`,
      },
    },
  },
};

const Template = ({
  allowDomainPunycode,
  allowLocalPartPunycode,
  allowDomainIp,
  allowStrictLocalPart,
  allowSpaces,
  allowName,
  allowLocalDomainName,
  ...rest
}) => {
  const [emailValue, setEmailValue] = useState("");

  const onChangeHandler = (value) => {
    setEmailValue(value);
  };
  const settings = {
    allowDomainPunycode,
    allowLocalPartPunycode,
    allowDomainIp,
    allowStrictLocalPart,
    allowSpaces,
    allowName,
    allowLocalDomainName,
  };
  return (
    <EmailInput
      {...rest}
      value={emailValue}
      emailSettings={settings}
      onValidateInput={(isEmailValid) => rest.onValidateInput(isEmailValid)}
      onChange={(e) => {
        rest.onChange(e.target.value);
        onChangeHandler(e.target.value);
      }}
    />
  );
};

export const Default = Template.bind({});
Default.args = {
  allowDomainPunycode: false,
  allowLocalPartPunycode: false,
  allowDomainIp: false,
  allowSpaces: false,
  allowName: false,
  allowLocalDomainName: false,
  allowStrictLocalPart: true,
  placeholder: "Input email",
  size: "base",
};
