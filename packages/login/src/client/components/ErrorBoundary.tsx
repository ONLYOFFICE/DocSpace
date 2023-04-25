import React from "react";
import ErrorContainer from "@docspace/common/components/ErrorContainer";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";
import { Dark, Base } from "@docspace/components/themes";

interface IError520Props {
  match?: {
    params: MatchType;
  };
  theme?: any;
}

interface IErrorBoundaryProps extends IError520Props {
  onError?: (error: any, errorInfo: any) => void;
  theme?: any;
  children?: React.ReactNode;
}

interface IErrorBoundaryState {
  hasError: boolean;
}

const Error520: React.FC<IError520Props> = ({ match }) => {
  const { t } = useTranslation(["Common"]);
  const params = useParams();
  const { error } = (params && params) || {};

  const theme =
    typeof window !== "undefined" &&
    window.matchMedia &&
    window.matchMedia("(prefers-color-scheme: dark)").matches
      ? Dark
      : Base;

  const themeProps = theme ? { theme } : {};

  return (
    <ErrorContainer
      headerText={t("SomethingWentWrong")}
      bodyText={error}
      {...themeProps}
    />
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
