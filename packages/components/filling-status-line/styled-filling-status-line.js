import styled from "styled-components";
import { Base } from "@docspace/components/themes";

const FillingStatusContainer = styled.div`
  width: 100%;
  max-width: 425px;
  padding: 10px;

  .status-done-text {
    color: ${(props) => (props.isDone ? "#4781D1" : "#A3A9AE")};
  }

  .status-done-icon {
    circle,
    path {
      stroke: ${(props) => (props.isDone ? "#4781D1" : "#A3A9AE")};
    }
  }

  .status-interrupted-text {
    color: ${(props) => props.isInterrupted && "#F2675A"};
  }

  .status-interrupted-icon {
    circle,
    path {
      stroke: ${(props) => props.isInterrupted && "#F2675A"};
    }
  }

  .status-done-icon,
  .status-interrupted-icon {
    margin-right: 10px;
  }
`;

const AccordionItem = styled.div`
  width: 100%;

  .accordion-item-info {
    display: flex;
    align-items: center;
    justify-content: space-between;
    cursor: pointer;
    height: 38px;
    padding: 18px 0;

    .user-avatar {
      padding: 1px;
      border: 2px solid #a3a9ae;
      border-color: ${(props) =>
        (props.isDone && "#4781D1") || (props.isInterrupted && "#F2675A")};
      border-radius: 50%;
    }

    .accordion-displayname {
      color: ${(props) => props.theme.color};
    }

    .accordion-role {
      color: ${(props) => (props.theme.isBase ? "#657077" : "#FFFFFF99")};
    }

    .arrow-icon {
      display: flex;
      align-items: center;
      justify-content: center;
      cursor: pointer;
      transform: ${(props) =>
        props.isOpen ? "rotate(270deg)" : "rotate(90deg)"};
      path {
        fill: ${(props) => (props.isOpen ? "#4781d1" : "#A3A9AE")};
      }
    }
  }

  .accordion-item-history {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding-left: 15px;
  }

  .accordion-item-wrapper {
    display: flex;
    align-items: center;
    min-height: 40px;
    margin: ${(props) => (props.isDone || props.isInterrupted ? "0" : "2px 0")};
    border-left: 2px
      ${(props) => (props.isDone || props.isInterrupted ? "solid" : "dashed")}
      #a3a9ae;
    border-color: ${(props) =>
      (props.isDone && "#4781D1") || (props.isInterrupted && "#F2675A")};

    .status-text {
      margin-left: 15px;
      color: ${(props) => (props.theme.isBase ? "#657077" : "#FFFFFF99")};
    }

    .filled-status-text {
      margin-left: 15px;
      color: ${(props) => (props.isDone ? "#4781D1" : "#657077")};
    }
  }

  .status-date {
    color: ${(props) => (props.theme.isBase ? "#657077" : "#FFFFFF99")};
  }
`;

AccordionItem.defaultProps = { theme: Base };

export { FillingStatusContainer, AccordionItem };
