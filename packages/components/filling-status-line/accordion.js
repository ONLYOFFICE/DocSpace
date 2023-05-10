import React, { useState } from "react";
import styled from "styled-components";

import ArrowReactSvgUrl from "PUBLIC_DIR/images/arrow.react.svg?url";

import IconButton from "../icon-button";
import Text from "../text";
import Box from "../box";
import Avatar from "../avatar";

const AccordionItem = styled.div`
  width: 100%;
  margin-bottom: 10px;
`;
const AccordionItemInfo = styled.div`
  display: flex;
  align-items: center;
  justify-content: space-between;
  cursor: pointer;
  height: 38px;

  .icon-button {
    display: flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
  }

  .icon-button-rotate {
    display: flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    transform: rotate(90deg);
  }
`;
const AccordionItemHistory = styled.div`
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 15px 5px 15px 15px;
`;

const StyledDivider = styled.div`
  height: 40px;
  width: 2px;
  background-color: #a3a9ae;
  margin-right: 28px;
`;

const Accordion = ({ avatar, displayName, role, status, date }) => {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <AccordionItem>
      <AccordionItemInfo onClick={() => setIsOpen(!isOpen)}>
        <Box displayProp="flex" alignItems="center">
          <Avatar
            className="user-avatar"
            size="min"
            role="user"
            source={avatar}
            userName={displayName}
          />

          <Box
            displayProp="flex"
            flexDirection="column"
            marginProp="0 0 0 10px"
          >
            <Text
              className="accordion-displayname"
              fontSize="14px"
              color="#333333"
              lineHeight="16px"
            >
              {displayName}
            </Text>
            <Text
              as="span"
              className="accordion-role"
              fontSize="12px"
              color="#657077"
              lineHeight="16px"
            >
              {role}
            </Text>
          </Box>
        </Box>
        <IconButton
          className={isOpen ? "icon-button-rotate" : "icon-button"}
          size={16}
          iconName={ArrowReactSvgUrl}
          isOpen={isOpen}
        />
      </AccordionItemInfo>
      {isOpen && (
        <AccordionItemHistory>
          <Box displayProp="flex" alignItems="center">
            <StyledDivider />
            <Text fontSize="12px" lineHeight="16px" color="#657077">
              {status}
            </Text>
          </Box>

          <Text fontSize="12px" lineHeight="16px" color="#657077">
            {date}
          </Text>
        </AccordionItemHistory>
      )}
    </AccordionItem>
  );
};
export default Accordion;
