import React from "react";
import styled from "styled-components";
import SelectedItem from "./";

export default {
  title: "Components/SelectedItem",
  component: SelectedItem,
  argTypes: {
    onClose: { action: "onClose" },
  },
};
const Template = ({ onClose, ...args }) => {
  return <SelectedItem {...args} onClose={(e) => onClose(e)} />;
};

export const Default = Template.bind({});
Default.args = {
  label: "Selected item",
  isInline: true,
  isDisabled: false,
};

const StyledContainer = styled.div`
  padding: 0;
  display: grid;
  grid-gap: 10px;
`;

const StyledContainerInline = styled.div`
  display: flex;
  flex-wrap: wrap;
  margin-bottom: 10px;

  > * {
    margin-right: 10px;
    margin-bottom: 10px;
  }
`;

const AllTemplate = ({ onClose, ...args }) => {
  const onCloseHandler = (e) => {
    onClose(e);
  };
  return (
    <>
      <StyledContainerInline>
        <SelectedItem
          label="Selected item"
          isInline={true}
          onClose={onCloseHandler}
        />
        <SelectedItem
          label="Selected item"
          isInline={true}
          isDisabled
          onClose={onCloseHandler}
        />
      </StyledContainerInline>

      <StyledContainer>
        <SelectedItem
          label="Selected item"
          isInline={false}
          onClose={onCloseHandler}
        />
      </StyledContainer>
    </>
  );
};

export const All = AllTemplate.bind({});
