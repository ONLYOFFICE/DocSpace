import React from "react";
import { Button, TextInput } from "@docspace/components";
import { observer } from "mobx-react";
import Text from "@docspace/components/text";
import { SpacesRowContainer } from "./RowView/SpacesRowContainer";
import { StyledMultipleSpaces } from "../StyledSpaces";
import { useStore } from "SRC_DIR/store";

const MultipleSpaces = () => {
  const { spacesStore } = useStore();

  const {
    portals,
    deletePortal,
    domain,
    setPortalName,
    setChangeDomainDialogVisible,
    setCreatePortalDialogVisible,
  } = spacesStore;

  return (
    <StyledMultipleSpaces>
      <div className="multiple-spaces-section">
        <Button
          size="small"
          label="New space"
          className="spaces-button"
          primary={true}
          onClick={() => setCreatePortalDialogVisible(true)}
        />
        <SpacesRowContainer portals={portals} />
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
            size="small"
            label="Edit"
            primary={true}
            onClick={() => setChangeDomainDialogVisible(true)}
          />
        </div>
      </div>
    </StyledMultipleSpaces>
  );
};

export default observer(MultipleSpaces);
