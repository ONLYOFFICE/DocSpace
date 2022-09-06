import styled from "styled-components";

const StyledInfoRoomBody = styled.div`
  padding-left: 20px;
  height: auto;
`;

const StyledTitle = styled.div`
  display: flex;
  flex-direction: row;
  align-items: center;
  width: 100%;
  height: 80px;
  gap: 8px;
`;

const StyledThumbnail = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  width: 100%;
  height: 200px;
  padding: 4px;
  margin-bottom: 24px;
  border: solid 1px #eceef1;
  border-radius: 6px;
`;

const StyledIcon = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  width: 100%;
  height: 96px;
`;

const StyledSubtitle = styled.div`
  padding: 24px 0;
  height: 64px;
  display: flex;
  align-items: center;
  justify-content: start;
`;

const StyledPropertiesTable = styled.div`
  display: flex;
  flex-direction: column;
  width: 100%;
  margin-bottom: 24px;
  gap: 8px;
`;

const StyledPropertyRow = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 110px 1fr;
  grid-column-gap: 24px;

  .property-title {
    font-size: 13px;
    color: #333333;
  }
`;

const StyledAccessRow = styled.div`
  width: 100%;
  height: 32px;
  display: flex;
  flex-direction: row;
  gap: 8px;
  align-items: center;

  .divider {
    background: #eceef1;
    margin: 2px 4px;
    width: 1px;
    height: 28px;
  }
`;

export {
  StyledInfoRoomBody,
  StyledSubtitle,
  StyledTitle,
  StyledThumbnail,
  StyledIcon,
  StyledPropertiesTable,
  StyledPropertyRow,
  StyledAccessRow,
};

const StyledItemAccess = styled.div`
  display: flex;
  flex-direction: row;
  gap: 8px;
  align-items: center;
  .divider {
    background: #eceef1;
    margin: 2px 4px;
    width: 1px;
    height: 28px;
  }
`;

const StyleditemAccessUser = styled.div`
  width: 32px;
  height: 32px;
  border-radius: 50%;

  a {
    img {
      border-radius: 50%;
      width: 100%;
      height: 100%;
    }
  }
`;
