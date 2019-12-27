import React from "react";
import styled from "styled-components";
import isEqual from "lodash/isEqual";
import { ComboBox, TextInput, IconButton } from "asc-web-components";

const Container = styled.div`
  display: flex;
  margin: 0 0 16px 0;

  .remove_icon {
    padding-left: 8px;
  }
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
      inputOnChange,

      removeButtonName,
      removeButtonOnChange
    } = this.props;

    return (
      <Container>
        <ComboBox
          name={comboBoxName}
          options={comboBoxOptions}
          onSelect={comboBoxOnChange}
          selectedOption={comboBoxSelectedOption}
          isDisabled={isDisabled}
          scaled={true}
          className="field-select"
        />
        <TextInput
          name={inputName}
          value={inputValue}
          isDisabled={isDisabled}
          onChange={inputOnChange}
        />
        <IconButton
          id={removeButtonName}
          className="remove_icon"
          size="16"
          onClick={removeButtonOnChange}
          iconName={"CatalogTrashIcon"}
          isFill={true}
          isClickable={true}
        />
      </Container>
    );
  }
}

export default ContactField;
