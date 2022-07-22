import React from "react";
import styled from "styled-components";
import equal from "fast-deep-equal/react";
import ContactField from "./ContactField";
import ComboBox from "@docspace/components/combobox";

const Container = styled.div`
  width: 100%;
  max-width: 450px;

  .field-select {
    width: 120px;
    margin: 0 8px 0 0;
  }
`;

const getOptions = (patterns, keyPrefix) => {
  return patterns.map((item, index) => {
    return {
      key: keyPrefix + index,
      label: item.type, //from resource
      icon: item.icon,
      value: item.type,
    };
  });
};

const renderItems = (
  contacts,
  pattern,
  onTypeChange,
  onTextChange,
  onRemove,
  isDisabled
) => {
  const items = contacts.map((contact, index) => {
    const prefix = contact.id + "_";
    const itemOptions = getOptions(pattern, prefix);
    const itemSelectedOption = itemOptions.filter(
      (option) => option.value === contact.type
    )[0];

    return (
      <ContactField
        key={prefix + "item_" + index}
        isDisabled={isDisabled}
        comboBoxName={prefix + "type"}
        comboBoxOptions={itemOptions}
        comboBoxSelectedOption={itemSelectedOption}
        comboBoxOnChange={onTypeChange}
        inputName={prefix + "value"}
        inputValue={contact.value}
        inputOnChange={onTextChange}
        removeButtonName={prefix + "remove"}
        removeButtonOnChange={onRemove}
      />
    );
  });

  return items;
};

class ContactsField extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !equal(this.props, nextProps);
  }

  render() {
    const {
      pattern,
      contacts,
      addItemText,
      onItemAdd,
      onItemTypeChange,
      onItemTextChange,
      onItemRemove,
      isDisabled,
    } = this.props;

    const existItems = renderItems(
      contacts,
      pattern,
      onItemTypeChange,
      onItemTextChange,
      onItemRemove,
      isDisabled
    );

    const prefix = "null_";
    const options = getOptions(pattern, prefix);
    const setDropDownMaxHeight =
      options && options.length > 6 ? { dropDownMaxHeight: 200 } : {};

    return (
      <Container>
        {existItems}
        <ComboBox
          options={options}
          onSelect={onItemAdd}
          scaledOptions={options.length < 6}
          selectedOption={{
            key: prefix,
            label: addItemText,
            value: "",
            default: true,
          }}
          isDisabled={isDisabled}
          scaled={true}
          className="field-select"
          directionY="both"
          {...setDropDownMaxHeight}
        />
      </Container>
    );
  }
}

export default ContactsField;
