import styled from "styled-components";

const StyledRow = styled.div`
  width: 688px;
  height: 69px;
  display: grid;
  grid-template-columns: repeat(11, 1fr);
  grid-template-rows: 18px 18px;
  grid-gap: 8px;

  grid-template-areas:
    "link date date date date date date . . .  options"
    ". comment comment comment comment . . .  . restore download";

  justify-items: center;
  align-items: center;

  @media (max-width: 1024px) {
    height: 67px;
    width: 100%;

    grid-template-areas:
      "link date date date date date date . . .  options"
      ". comment comment comment comment . . . . . .";
    .history-loader-restore-btn,
    .history-loader-download-btn {
      display: none;
    }

    .history-loader-options {
      width: 16px;
    }
  }
  .history-loader-file-link {
    min-width: 54px;
    grid-area: link;
  }

  .history-loader-file-date {
    grid-area: date;
  }

  .history-loader-options {
    grid-area: options;
  }

  .history-loader-comment {
    grid-area: comment;
  }

  .history-loader-restore-btn {
    grid-area: restore;
  }

  .history-loader-download-btn {
    grid-area: download;
  }

  .history-loader-comment,
  .history-loader-file-date {
    margin-left: 16px;
  }
  .history-loader-options,
  .history-loader-restore-btn,
  .history-loader-download-btn {
    justify-self: end;
  }
`;

export default StyledRow;
