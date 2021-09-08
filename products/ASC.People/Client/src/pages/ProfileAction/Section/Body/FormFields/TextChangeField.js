import React from "react";
import styled from "styled-components";
import equal from "fast-deep-equal/react";
import FieldContainer from "@appserver/components/field-container";
import TextInput from "@appserver/components/text-input";
import Button from "@appserver/components/button";

const InputContainer = styled.div`
  width: 100%;
  max-width: 320px;
  display: flex;
  align-items: center;
`;

class TextChangeField extends React.Component {
  shouldComponentUpdate(nextProps) {
    return !equal(this.props, nextProps);
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
      dataDialog,
      maxLabelWidth,
    } = this.props;

    return (
      <FieldContainer
        isRequired={isRequired}
        hasError={hasError}
        labelText={labelText}
        tooltipContent={tooltipContent}
        helpButtonHeaderContent={helpButtonHeaderContent}
        maxLabelWidth={maxLabelWidth}
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
