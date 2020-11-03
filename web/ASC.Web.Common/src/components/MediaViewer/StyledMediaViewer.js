import styled from "styled-components";

const StyledMediaViewer = styled.div`
  color: #d1d1d1;
  display: ${(props) => (props.visible ? "block" : "none")};
  overflow: hidden;
  .videoViewerOverlay {
    position: fixed;
    z-index: 4000;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: black;
    opacity: 0.5;
  }
  .mediaViewerToolbox {
    z-index: 4001;
    padding-top: 14px;
    padding-bottom: 14px;
    height: 20px;
    width: 100%;
    background-color: rgba(11, 11, 11, 0.7);
    position: fixed;
    bottom: 0;
    left: 0;
    text-align: center;
  }
  span {
    position: fixed;
    right: 0;
    bottom: 5px;
    margin-right: 10px;
    z-index: 4005;
  }
  .deleteBtnContainer,
  .downloadBtnContainer {
    display: block;
    width: 20px;
    margin: 3px 10px;
    line-height: 19px;
  }
  .details {
    z-index: 4001;
    padding-top: 14px;
    padding-bottom: 14px;
    height: 20px;
    width: 100%;
    background: rgba(17, 17, 17, 0.867);
    position: fixed;
    top: 0;
    left: 0;

    .title {
      text-align: center;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }
  }

  .mediaPlayerClose {
    position: fixed;
    top: 4px;
    right: 10px;
    height: 30px;
  }
`;

export default StyledMediaViewer;
