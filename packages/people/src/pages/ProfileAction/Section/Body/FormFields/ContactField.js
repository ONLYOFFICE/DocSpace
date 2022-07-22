import React from "react";
import styled from "styled-components";
import equal from "fast-deep-equal/react";
import ComboBox from "@docspace/components/combobox";
import TextInput from "@docspace/components/text-input";
import IconButton from "@docspace/components/icon-button";

const Container = styled.div`
  display: flex;
  margin: 0 0 16px 0;
  align-items: center;

  .remove_icon {
    padding-left: 8px;
  }
`;

class ContactField extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !equal(this.props, nextProps);
  }

  render() {
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
      removeButtonOnChange,
    } = this.props;

    const setDropDownMaxHeight =
      comboBoxOptions && comboBoxOptions.length > 6
        ? { dropDownMaxHeight: 200 }
        : {};

    return (
      <Container>
        <ComboBox
          name={comboBoxName}
          options={comboBoxOptions}
          onSelect={comboBoxOnChange}
          selectedOption={comboBoxSelectedOption}
          isDisabled={isDisabled}
          scaled={true}
          directionY="both"
          className="field-select"
          scaledOptions={comboBoxOptions.length < 7}
          {...setDropDownMaxHeight}
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
          iconName={"/static/images/catalog.trash.react.svg"}
          isFill={true}
          isClickable={true}
        />
      </Container>
    );
  }
}

export default ContactField;
