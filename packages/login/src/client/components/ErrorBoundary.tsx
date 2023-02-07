import React from "react";
import ErrorContainer from "@docspace/common/components/ErrorContainer";
import { useTranslation } from "react-i18next";

interface IError520Props {
  match?: {
    params: MatchType;
  };
}

interface IErrorBoundaryProps extends IError520Props {
  onError?: (error: any, errorInfo: any) => void;
  children?: React.ReactNode;
}

interface IErrorBoundaryState {
  hasError: boolean;
}

const Error520: React.FC<IError520Props> = ({ match }) => {
  const { t } = useTranslation(["Common"]);
  const { error } = (match && match.params) || {};

  return (
    <ErrorContainer headerText={t("SomethingWentWrong")} bodyText={error} />
  );
};

class ErrorBoundary extends React.Component<
  IErrorBoundaryProps,
  IErrorBoundaryState
> {
  state: IErrorBoundaryState = { hasError: false };
  props: any;
  // eslint-disable-next-line no-unused-vars
  static getDerivedStateFromError() {
    // Update state so the next render will show the fallback UI.
    return { hasError: true };
  }

  componentDidCatch(error: any, errorInfo: any) {
    // You can also log the error to an error reporting service
    console.error(error, errorInfo);
    this.props.onError && this.props.onError(error, errorInfo);
  }

  render() {
    if (this.state.hasError) {
      // You can render any custom fallback UI
      return <Error520 {...this.props} />;
    }

    return this.props.children;
  }
}

export default ErrorBoundary;
