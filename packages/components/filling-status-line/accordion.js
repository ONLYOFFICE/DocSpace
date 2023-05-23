import React, { useState } from "react";
import styled from "styled-components";

import ArrowReactSvgUrl from "PUBLIC_DIR/images/arrow.react.svg?url";

import IconButton from "../icon-button";
import Text from "../text";
import Box from "../box";
import Avatar from "../avatar";

const AccordionItem = styled.div`
  width: 100%;
`;
const AccordionItemInfo = styled.div`
  display: flex;
  align-items: center;
  justify-content: space-between;
  cursor: pointer;
  height: 38px;
  padding: 18px 0;

  .user-avatar {
    padding 1px;
    border: 2px solid ${(props) => (props.finished ? "#4781D1" : "#A3A9AE")};
    border-radius: 50%;
  }

  .icon-button {
    display: flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    transform: rotate(90deg);
  }

  .icon-button-rotate {
    path {
      fill: #4781d1;
    }
    display: flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    transform: rotate(270deg);
  }
`;

const AccordionItemHistory = styled.div`
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding-left: 15px;
`;

const AccordionItemDetailHistory = styled.div`
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding-left: 15px;
`;

const ItemWrapper = styled.div`
  display: flex;
  align-items: center;
  border-left: 2px ${(props) => (props.finished ? "solid" : "dashed")};
  border-color: ${(props) => (props.finished ? "#4781D1" : "#A3A9AE")};
  min-height: 40px;
  margin: ${(props) => (props.finished ? "0" : "2px 0")};

  .filled-status-text {
    color: ${(props) => (props.finished ? "#4781D1" : "#657077")};
  }
`;

const Accordion = ({
  avatar,
  displayName,
  role,
  startFillingStatus,
  startFillingDate,
  filledAndSignedStatus,
  filledAndSignedDate,
  returnedByUser,
  returnedByUserDate,
  comment,
  finished,
}) => {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <AccordionItem>
      <AccordionItemInfo finished={finished} onClick={() => setIsOpen(!isOpen)}>
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
              fontWeight="bold"
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
        />
      </AccordionItemInfo>

      <AccordionItemHistory>
        <ItemWrapper finished={finished}>
          <Text
            fontSize="12px"
            lineHeight="16px"
            className="filled-status-text"
            style={{ marginLeft: "15px" }}
          >
            {filledAndSignedStatus}
          </Text>
        </ItemWrapper>

        <Text fontSize="12px" lineHeight="16px" color="#657077">
          {filledAndSignedDate}
        </Text>
      </AccordionItemHistory>

      {isOpen && (
        <>
          <AccordionItemDetailHistory>
            <ItemWrapper finished={finished}>
              <Text
                fontSize="12px"
                lineHeight="16px"
                color="#657077"
                className="status-text"
                style={{ marginLeft: "15px" }}
              >
                {startFillingStatus}
              </Text>
            </ItemWrapper>

            <Text fontSize="12px" lineHeight="16px" color="#657077">
              {startFillingDate}
            </Text>
          </AccordionItemDetailHistory>

          {returnedByUser && (
            <AccordionItemDetailHistory>
              <ItemWrapper finished={finished}>
                <Text
                  fontSize="12px"
                  lineHeight="16px"
                  color="#657077"
                  className="status-text"
                  style={{ marginLeft: "15px" }}
                >
                  {returnedByUser}
                </Text>
              </ItemWrapper>

              <Text fontSize="12px" lineHeight="16px" color="#657077">
                {returnedByUserDate}
              </Text>
            </AccordionItemDetailHistory>
          )}

          {comment && (
            <AccordionItemDetailHistory>
              <ItemWrapper finished={finished}>
                <Text
                  fontSize="12px"
                  lineHeight="16px"
                  color="#657077"
                  className="status-text"
                  style={{ marginLeft: "15px" }}
                >
                  {comment}
                </Text>
              </ItemWrapper>
            </AccordionItemDetailHistory>
          )}
        </>
      )}
    </AccordionItem>
  );
};
export default Accordion;
