import React from "react";
import PropTypes from "prop-types";
import ErrorContainer from "@docspace/common/components/ErrorContainer";
import { useParams } from "react-router-dom";
import { useTranslation } from "react-i18next";

const Error520 = ({ match }) => {
  const { t } = useTranslation(["Common"]);
  const params = useParams();
  const { error } = (params && params) || {};

  return (
    <ErrorContainer headerText={t("SomethingWentWrong")} bodyText={error} />
  );
};

class ErrorBoundary extends React.Component {
  constructor(props) {
    super(props);
    this.state = { hasError: false };
  }
  // eslint-disable-next-line no-unused-vars
  static getDerivedStateFromError(error) {
    // Update state so the next render will show the fallback UI.
    return { hasError: true };
  }

  componentDidCatch(error, errorInfo) {
    // You can also log the error to an error reporting service
    console.error(error, errorInfo);
    this.props.onError && this.props.onError();
  }

  render() {
    if (this.state.hasError) {
      // You can render any custom fallback UI
      return <Error520 />;
    }

    return this.props.children;
  }
}

ErrorBoundary.propTypes = {
  children: PropTypes.any,
  onError: PropTypes.func,
};

export default ErrorBoundary;
