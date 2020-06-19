import styled from 'styled-components';

const StyledSettingsWrapper = styled.div`
  display: flex;
  flex-wrap: wrap;
  align-items: top;
  margin-top: 26px;
  width: 100%;

  @media(max-width: 600px) {
    margin-top: 0px;
    flex-direction: column;
  }

  .float-right {
    @media(max-width: 768px) {
      width: 50%;
    }

    @media(max-width: 600px) {
      margin-left: 0;
      width: 100%;
    }
    width: 70%;
  }
`;

export default StyledSettingsWrapper;