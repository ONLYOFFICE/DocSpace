import React from "react";
import styled from "styled-components";
import Heading from "../../heading";
import Text from "../../text";

const StyledContainer = styled.div`
  width: 100%;

  display: flex;
  align-items: center;
  flex-direction: column;

  margin-top: ${(props) => (props.withSearch ? "80px" : "64px")};
  padding: 0 28px;

  box-sizing: border-box;

  .empty-image {
    max-width: 72px;
    max-height: 72px;

    margin-bottom: 32px;
  }

  .empty-header {
    font-weight: 700;
    font-size: 16px;
    line-height: 22px;

    margin: 0;
  }

  .empty-description {
    font-weight: 400;
    font-size: 12px;
    line-height: 16px;

    text-align: center;

    color: #555f65;

    margin-top: 8px;
  }
`;

const EmptyScreen = ({
  image,
  header,
  description,
  searchImage,
  searchHeader,
  searchDescription,
  withSearch,
}) => {
  const currentImage = withSearch ? searchImage : image;
  const currentHeader = withSearch ? searchHeader : header;
  const currentDescription = withSearch ? searchDescription : description;

  return (
    <StyledContainer withSearch={withSearch}>
      <img
        className="empty-image"
        src={currentImage}
        alt="empty-screen-image"
      />
      <Heading level={3} className="empty-header">
        {currentHeader}
      </Heading>
      <Text className="empty-description" noSelect>
        {currentDescription}
      </Text>
    </StyledContainer>
  );
};

export default EmptyScreen;
