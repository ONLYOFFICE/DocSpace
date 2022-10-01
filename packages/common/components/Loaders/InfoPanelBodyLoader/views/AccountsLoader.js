import React from "react";
import styled from "styled-components";

import RectangleLoader from "../../RectangleLoader/RectangleLoader";

const StyledAccountsLoader = styled.div`
  width: 100%;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: start;
`;

const StyledSubtitleLoader = styled.div`
  width: 100%;
  padding: 24px 0 24px 0;
  display: flex;
  flex-direction: row;
  justify-content: space-between;
  align-items: center;
`;

const StyledProperty = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 108px 1fr;
  grid-column-gap: 24px;
  grid-row-gap: 8px;

  .property-content {
    max-width: 100%;
    margin: auto 0;

    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }
`;

const propertyDimensions = [
  { propertyTitle: "50px", propertyContent: "39px" },
  { propertyTitle: "29px", propertyContent: "64px" },
  { propertyTitle: "38px", propertyContent: "28px" },
  { propertyTitle: "36px", propertyContent: "120px" },
  { propertyTitle: "0px", propertyContent: "160px" },
];

const AccountsLoader = () => {
  return (
    <StyledAccountsLoader>
      <StyledSubtitleLoader>
        <RectangleLoader width={"71px"} height={"16px"} borderRadius={"3px"} />
      </StyledSubtitleLoader>

      <StyledProperty>
        {propertyDimensions.map((property) => [
          <RectangleLoader
            className="property-title"
            width={property.propertyTitle}
            height={"20px"}
            borderRadius={"3px"}
          />,
          <RectangleLoader
            className="property-content"
            width={property.propertyContent}
            height={"20px"}
            borderRadius={"3px"}
          />,
        ])}
      </StyledProperty>
    </StyledAccountsLoader>
  );
};

export default AccountsLoader;
