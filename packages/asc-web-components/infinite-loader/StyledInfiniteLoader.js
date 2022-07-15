import styled from "styled-components";

const StyledTableLoader = styled.div`
  grid-column-start: 1;
  grid-column-end: -1;
  display: grid;
  padding-top: 16px;
`;

const StyledRowLoader = styled.div`
  padding-top: 16px;
`;

const StyledCard = styled.div`
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(216px, 1fr));

  //gap: 14px 16px;
`;

const StyledItem = styled.div`
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(216px, 1fr));
  gap: 14px 16px;
`;

export { StyledTableLoader, StyledRowLoader, StyledCard, StyledItem };
