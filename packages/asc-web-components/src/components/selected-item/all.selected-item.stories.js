import React from "react";
import { storiesOf } from "@storybook/react";
import SelectedItem from ".";
import Section from "../../../.storybook/decorators/section";
import styled from "@emotion/styled";

function onClose(e) {
  console.log("onClose", e);
}

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

storiesOf("Components|SelectedItem", module)
  .addParameters({ options: { showAddonPanel: false } })
  .add("all", () => {
    return (
      <Section>
        <StyledContainerInline>
          <SelectedItem
            text="Selected item"
            isInline={true}
            onClose={onClose}
          />
          <SelectedItem
            text="Selected item"
            isInline={true}
            onClose={onClose}
          />
          <SelectedItem
            text="Selected item"
            isInline={true}
            onClose={onClose}
          />
          <SelectedItem
            text="Selected item"
            isInline={true}
            onClose={onClose}
          />
        </StyledContainerInline>

        <StyledContainer>
          <SelectedItem
            text="Selected item"
            isInline={false}
            onClose={onClose}
          />
          <SelectedItem
            text="Selected item"
            isInline={false}
            onClose={onClose}
          />
          <SelectedItem
            text="Selected item"
            isInline={false}
            onClose={onClose}
          />
          <SelectedItem
            text="Selected item"
            isInline={false}
            onClose={onClose}
          />
        </StyledContainer>
      </Section>
    );
  });
