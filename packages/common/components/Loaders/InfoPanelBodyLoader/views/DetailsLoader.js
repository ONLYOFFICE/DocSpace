import React from "react";
import styled from "styled-components";

import RectangleLoader from "../../RectangleLoader/RectangleLoader";

const StyledDetailsLoader = styled.div`
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
  grid-template-columns: 101px 1fr;
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
  { propertyTitle: "41px", propertyContent: "160px" },
  { propertyTitle: "52px", propertyContent: "120px" },
  { propertyTitle: "29px", propertyContent: "89px" },
  { propertyTitle: "84px", propertyContent: "23px" },
  { propertyTitle: "24px", propertyContent: "47px" },
  { propertyTitle: "86px", propertyContent: "127px" },
  { propertyTitle: "101px", propertyContent: "160px" },
  { propertyTitle: "82px", propertyContent: "127px" },
  { propertyTitle: "52px", propertyContent: "32px" },
  { propertyTitle: "67px", propertyContent: "43px" },
];

const DetailsLoader = () => {
  return (
    <StyledDetailsLoader>
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
    </StyledDetailsLoader>
  );
};

export default DetailsLoader;
