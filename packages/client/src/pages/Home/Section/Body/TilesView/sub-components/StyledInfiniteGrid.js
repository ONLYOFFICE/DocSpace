import styled from "styled-components";

const StyledCard = styled.div`
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(216px, 1fr));
  height: ${({ cardHeight }) => `${cardHeight}px`};
`;

const StyledItem = styled.div`
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(216px, 1fr));
  gap: 14px 16px;
`;

const StyledHeaderItem = styled.div`
  height: 20px;
  grid-column: -1 / 1;
`;

export { StyledCard, StyledItem, StyledHeaderItem };
