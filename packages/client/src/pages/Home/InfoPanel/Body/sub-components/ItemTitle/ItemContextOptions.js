import React from "react";
import styled from "styled-components";
import { inject } from "mobx-react";
import { withTranslation } from "react-i18next";

import { ContextMenuButton } from "@docspace/components";

import ContextHelper from "../../helpers/ContextHelper";

const StyledItemContextOptions = styled.div`
  margin-left: auto;
`;

const ItemContextOptions = ({ selection, setBufferSelection, ...props }) => {
  if (!selection) return null;

  const contextHelper = new ContextHelper({
    selection,
    ...props,
  });

  const setItemAsBufferSelection = () => setBufferSelection(selection);

  return (
    <StyledItemContextOptions onClick={setItemAsBufferSelection}>
      <ContextMenuButton
        zIndex={402}
        className="option-button expandButton"
        iconName="images/vertical-dots.react.svg"
        size={15}
        getData={contextHelper.getItemContextOptions}
        directionX="left"
        displayType="dropdown"
        isPortal
      />
    </StyledItemContextOptions>
  );
};

export default ItemContextOptions;
