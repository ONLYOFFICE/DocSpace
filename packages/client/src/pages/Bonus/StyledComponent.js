import styled from "styled-components";

const StyledComponent = styled.div`
  max-width: 660px;
  .official-documentation {
    margin: 12px 0;
    display: grid;
    grid-template-columns: 20px 1fr;
    grid-template-rows: 1fr 1fr 1fr;
    a {
      text-decoration: underline;
    }
  }
  .upgrade-info {
    margin-bottom: 24px;
  }
`;

export default StyledComponent;
