import React from "react";
import { inject, observer } from "mobx-react";
import styled, { css } from "styled-components";

import ModalDialog from "@docspace/components/modal-dialog";
import Avatar from "@docspace/components/avatar";
import Text from "@docspace/components/text";
import SelectorAddButton from "@docspace/components/selector-add-button";
import Button from "@docspace/components/button";
import { Base } from "@docspace/components/themes";

import PeopleSelector from "SRC_DIR/components/PeopleSelector";

const StyledOwnerInfo = styled.div`
  display: flex;
  align-items: center;
  justify-content: start;

  margin-top: 8px;
  margin-bottom: 24px;

  .info {
    padding-left: 16px;
    display: flex;
    flex-direction: column;

    .display-name {
      font-weight: 700;
      font-size: 16px;
      line-height: 22px;
    }

    .owner {
      font-weight: 600;
      font-size: 13px;
      line-height: 20px;
      color: ${(props) => props.theme.text.disableColor};
    }
  }
`;

const StyledPeopleSelectorInfo = styled.div`
  margin-bottom: 12px;

  .new-owner {
    font-weight: 600;
    font-size: 15px;
    line-height: 16px;
    margin-bottom: 4px;
  }

  .description {
    font-weight: 400;
    font-size: 13px;
    line-height: 20px;

    color: #a3a9ae;
  }
`;

StyledOwnerInfo.defaultProps = { theme: Base };

const StyledPeopleSelector = styled.div`
  display: flex;
  align-items: center;

  margin-bottom: 24px;

  .label {
    font-weight: 600;
    font-size: 13px;
    line-height: 20px;

    color: #a3a9ae;

    margin-left: 8px;
  }
`;

StyledPeopleSelector.defaultProps = { theme: Base };

const StyledAvailableList = styled.div`
  display: flex;

  flex-direction: column;

  margin-bottom: 24px;

  .list-header {
    font-weight: 600;
    font-size: 13px;
    line-height: 20px;

    margin-bottom: 8px;
  }

  .list-item {
    font-weight: 400;
    font-size: 13px;
    line-height: 20px;

    margin-bottom: 2px;
  }
`;

StyledAvailableList.defaultProps = { theme: Base };

const StyledFooterWrapper = styled.div`
  height: 100%;
  width: 100%;

  display: flex;

  flex-direction: column;

  .info {
    margin-bottom: 16px;

    font-weight: 400;
    font-size: 13px;
    line-height: 20px;
  }

  .button-wrapper {
    display: flex;
    align-items: center;
    justify-content: space-between;

    flex-direction: row;

    gap: 8px;
  }
`;

StyledFooterWrapper.defaultProps = { theme: Base };

const ChangePortalOwnerDialog = ({ isOwner, displayName, avatar }) => {
  const [selectorVisible, setSelectorVisible] = React.useState(false);
  const [isLoading, setIsLoading] = React.useState(false);
  const [isVisible, setIsVisible] = React.useState(true);

  const onTogglePeopleSelector = () => {
    setSelectorVisible((val) => !val);
  };

  const onSelectOption = (e) => {};

  const onCreate = () => {};

  const onClose = () => {
    setIsVisible(false);
  };

  const availableList = [
    "Do the same as administrators",
    "Appoint administrators",
    "Set access rights",
    "Manage portal configuration",
    "Manage user accounts",
    "Change portal owner",
    "Backup portal data",
    "Deactivate or delete portal",
  ];

  console.log(selectorVisible);

  return (
    <>
      {selectorVisible ? (
        <PeopleSelector
          className="people-selector"
          isOpen={selectorVisible}
          isMultiSelect={false}
          onSelect={onSelectOption}
          onArrowClick={onTogglePeopleSelector}
          headerLabel={"Choose owner"}
        />
      ) : (
        <ModalDialog
          displayType={"aside"}
          visible={isVisible}
          onClose={onClose}
          withFooterBorder
        >
          <ModalDialog.Header>Change type</ModalDialog.Header>
          <ModalDialog.Body>
            <StyledOwnerInfo>
              <Avatar
                className="avatar"
                role={"owner"}
                source={avatar}
                size={"big"}
              />
              <div className="info">
                <Text className="display-name" noSelect title={displayName}>
                  {displayName}
                </Text>
                <Text className="owner" noSelect title={"Owner"}>
                  Owner
                </Text>
              </div>
            </StyledOwnerInfo>
            <StyledPeopleSelectorInfo>
              <Text className="new-owner" noSelect title={"New portal owner"}>
                New portal owner
              </Text>
              <Text className="description" noSelect title={displayName}>
                To change the Portal owner please choose the Name of the new
                Portal owner below
              </Text>
            </StyledPeopleSelectorInfo>
            <StyledPeopleSelector>
              <SelectorAddButton
                className="selector-add-button"
                onClick={onTogglePeopleSelector}
              />
              <Text className="label" noSelect title={"Choose owner"}>
                Choose owner
              </Text>
            </StyledPeopleSelector>
            <StyledAvailableList>
              <Text className="list-header" noSelect title={"Portal owner can"}>
                Portal owner can:
              </Text>

              {availableList.map((item) => (
                <Text key={item} className="list-item" noSelect title={item}>
                  — {item}
                </Text>
              ))}
            </StyledAvailableList>
          </ModalDialog.Body>
          <ModalDialog.Footer>
            <StyledFooterWrapper>
              <Text className="info">
                Changes will be applied after the confirmation via email
              </Text>
              <div className="button-wrapper">
                <Button
                  tabIndex={5}
                  label={"Create"}
                  size="normal"
                  primary
                  scale
                  onClick={onCreate}
                  isLoading={isLoading}
                />
                <Button
                  tabIndex={5}
                  label={"Cancel"}
                  size="normal"
                  scale
                  onClick={onClose}
                />
              </div>
            </StyledFooterWrapper>
          </ModalDialog.Footer>
        </ModalDialog>
      )}
    </>
  );
};

export default inject(({ auth }) => {
  const { isOwner, displayName, avatar } = auth.userStore.user;
  console.log(auth.userStore.user);
  return { isOwner, displayName, avatar };
})(observer(ChangePortalOwnerDialog));
