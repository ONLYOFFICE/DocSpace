import React from 'react'
import styled from 'styled-components';
import { ComboBox, TextInput } from 'asc-web-components'

const Container = styled.div`
  width: 450px;

  .field-select {
    width: 120px;
    margin: 0 8px 0 0;
  }
`;

const Item = styled.div`
  display: flex;
  margin: 0 0 16px 0;
`;

const getOptions = (patterns, keyPrefix) => {
    return patterns.map((item, index) => {
        return {
            key: keyPrefix + index,
            label: item.type, //from resource
            icon: item.icon,
            value: item.type
        };
    });
}

const renderItems = (contacts, pattern, onTypeChange, onTextChange, isDisabled) => {
    const items = contacts.map((contact, index) => {

        const prefix = contact.id + "_";

        const itemOptions = getOptions(pattern, prefix);

        const itemSelectedOption = itemOptions.filter(option => option.value === contact.type)[0];
        
        return (
            <Item key={prefix + "item_" + index}>
                <ComboBox
                    name={prefix + "type"}
                    options={itemOptions}
                    onSelect={onTypeChange}
                    selectedOption={itemSelectedOption}
                    isDisabled={isDisabled}
                    scaled={false}
                    className="field-select"
                />
                <TextInput
                    name={prefix + "value"}
                    value={contact.value}
                    isDisabled={isDisabled}
                    onChange={onTextChange}
                />
            </Item>
        );
    });
  
    return items;
  };

const ContactsField = React.memo((props) => {

    const { pattern, contacts, addItemText, onItemAdd, onItemTypeChange, onItemTextChange, isDisabled } = props;

    const existItems = renderItems(contacts, pattern, onItemTypeChange, onItemTextChange, isDisabled);

    const prefix = "null_";

    const options = getOptions(pattern, prefix);

    return (
        <Container>
            {existItems}
            <ComboBox
                options={options}
                onSelect={onItemAdd}
                selectedOption={{
                    key: prefix,
                    label: addItemText,
                    value: ""
                }}
                isDisabled={isDisabled}
                scaled={false}
                className="field-select"
            />
        </Container>
    );
});

export default ContactsField