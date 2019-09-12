import React from "react";
import styled from "styled-components";
import isEqual from "lodash/isEqual";
import { ComboBox, TextInput } from "asc-web-components";

const Container = styled.div`
  display: flex;
  margin: 0 0 16px 0;
`;

class ContactField extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !isEqual(this.props, nextProps);
  }

  render() {
    console.log("ContactField render");

    const {
      isDisabled,

      comboBoxName,
      comboBoxOptions,
      comboBoxSelectedOption,
      comboBoxOnChange,

      inputName,
      inputValue,
      inputOnChange
    } = this.props;

    return (
      <Container>
        <ComboBox
          name={comboBoxName}
          options={comboBoxOptions}
          onSelect={comboBoxOnChange}
          selectedOption={comboBoxSelectedOption}
          isDisabled={isDisabled}
          scaled={false}
          className="field-select"
        />
        <TextInput
          name={inputName}
          value={inputValue}
          isDisabled={isDisabled}
          onChange={inputOnChange}
        />
      </Container>
    );
  }
}

export default ContactField;
