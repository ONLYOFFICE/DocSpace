import styled from "styled-components";

const StyledNoItemContainer = styled.div`
  margin: 80px auto 0;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-direction: column;
  gap: 32px;

  .no-item-text {
    font-weight: 600;
    font-size: 14px;
    line-height: 16px;
    text-align: center;
  }

  .no-thumbnail-img-wrapper {
    width: 75px;
    height: 75px;
    img {
      width: 75px;
      height: 75px;
    }
  }
`;

export { StyledNoItemContainer };
