import React from "react";
import styled from "styled-components";
import isEqual from "lodash/isEqual";
import { FieldContainer, TextInput, Button } from "asc-web-components";

const InputContainer = styled.div`
  width: 100%;
  max-width: 320px;
  display: flex;
  align-items: center;
`;

class TextChangeField extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !isEqual(this.props, nextProps);
  }

  render() {
    console.log("TextChangeField render");

    const {
      isRequired,
      hasError,
      labelText,

      inputName,
      inputValue,
      inputTabIndex,

      buttonText,
      buttonIsDisabled,
      buttonOnClick,
      buttonTabIndex,

      tooltipContent,
      helpButtonHeaderContent,
      dataDialog
    } = this.props;

    return (
      <FieldContainer
        isRequired={isRequired}
        hasError={hasError}
        labelText={labelText}
        tooltipContent={tooltipContent}
        helpButtonHeaderContent={helpButtonHeaderContent}
      >
        <InputContainer>
          <TextInput
            name={inputName}
            value={inputValue}
            isDisabled={true}
            hasError={hasError}
            tabIndex={inputTabIndex}
          />
          <Button
            label={buttonText}
            onClick={buttonOnClick}
            isDisabled={buttonIsDisabled}
            size="medium"
            style={{ marginLeft: "8px" }}
            tabIndex={buttonTabIndex}
            data-dialog={dataDialog}
          />
        </InputContainer>
      </FieldContainer>
    );
  }
}

export default TextChangeField;
