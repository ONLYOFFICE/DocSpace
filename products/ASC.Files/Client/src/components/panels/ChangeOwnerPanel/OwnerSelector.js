import React from "react";
import PeopleSelector from "people/PeopleSelector";
import Aside from "@appserver/components/aside";
import Backdrop from "@appserver/components/backdrop";
import Heading from "@appserver/components/heading";
import IconButton from "@appserver/components/icon-button";
import {
  StyledAddUsersPanelPanel,
  StyledContent,
  StyledHeaderContent,
  StyledBody,
} from "../StyledPanels";

const OwnerSelector = (props) => {
  const {
    ownerLabel,
    isOpen,
    groupsCaption,
    onOwnerSelect,
    onClose,
    onClosePanel,
  } = props;

  const zIndex = 310;

  return (
    <StyledAddUsersPanelPanel visible={isOpen}>
      <Backdrop
        onClick={onClose}
        visible={isOpen}
        zIndex={zIndex}
        isAside={true}
      />
      <Aside className="header_aside-panel">
        <StyledContent>
          <StyledHeaderContent>
            <IconButton
              size="16"
              iconName="/static/images/arrow.path.react.svg"
              onClick={onClosePanel}
              color="#A3A9AE"
            />
            <Heading
              className="header_aside-panel-header"
              size="medium"
              truncate
            >
              {ownerLabel}
            </Heading>
          </StyledHeaderContent>

          <StyledBody /*ref={this.scrollRef}*/>
            <PeopleSelector
              role="user"
              employeeStatus={1}
              displayType="aside"
              withoutAside
              isOpen={isOpen}
              onSelect={onOwnerSelect}
              groupsCaption={groupsCaption}
              onCancel={onClose}
            />
          </StyledBody>
        </StyledContent>
      </Aside>
    </StyledAddUsersPanelPanel>
  );
};

export default OwnerSelector;
