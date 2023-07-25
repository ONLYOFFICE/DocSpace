import React from "react";
import { Button, TextInput } from "@docspace/components";
import { inject, observer } from "mobx-react";
import styled from "styled-components";
import Text from "@docspace/components/text";
import { SpacesRowContainer } from "./RowView/SpacesRowContainer";
import { StyledMultipleSpaces } from "../StyledSpaces";
import { useStore } from "SRC_DIR/store";

const MultipleSpaces = () => {
  // const {
  //   setCreateDocspaceDialogVisible,
  //   setChangeDomainDialogVisible,
  //   deletePortal,
  //   portals,
  //   domain,
  // } = props;

  const { spacesStore } = useStore();

  const { portals, deletePortal, domain } = spacesStore;

  return (
    <StyledMultipleSpaces>
      <div className="multiple-spaces-section">
        <Button
          size="normal"
          label="New space"
          className="spaces-button"
          primary={true}
          //   onClick={() => setCreateDocspaceDialogVisible(true)}
        />
        <SpacesRowContainer deletePortal={deletePortal} portals={portals} />
        <div className="domain-settings-wrapper">
          <Text fontSize="16px" fontWeight={700}>
            Domain settings
          </Text>

          <div className="spaces-input-block">
            <Text
              color="#333"
              fontSize="13px"
              className="spaces-input-subheader"
              fontWeight="600"
            >
              Your current domain
            </Text>
            <TextInput
              isDisabled={true}
              placeholder={domain}
              className="spaces-input"
            />
          </div>

          <Button
            className="spaces-button"
            size="normal"
            label="Edit"
            primary={true}
            //    onClick={() => setChangeDomainDialogVisible(true)}
          />
        </div>
      </div>
    </StyledMultipleSpaces>
  );
};

// export default inject(({ dialogsStore, auth }) => {
//   const {
//     setCreateDocspaceDialogVisible,
//     setChangeDomainDialogVisible,
//   } = dialogsStore;

//   const {
//     getSpaces,
//     portals,
//     domain,
//     deletePortal,
//   } = auth.spaceManagementStore;

//   return {
//     setChangeDomainDialogVisible,
//     setCreateDocspaceDialogVisible,
//     deletePortal,
//     getSpaces,
//     portals,
//     domain,
//   };
// })(observer(MultipleSpaces));

export default observer(MultipleSpaces);
