import styled from "styled-components";

const StyledNoItemContainer = styled.div`
  margin: 80px auto;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-direction: column;
  gap: 32px;

  .no-item-text {
    font-family: "Open Sans";
    font-style: normal;
    font-weight: 600;
    font-size: 14px;
    line-height: 16px;
    text-align: center;
  }

  .no-thumbnail-img-wrapper {
    width: 96px;
    height: 100px;
  }
`;

export { StyledNoItemContainer };
