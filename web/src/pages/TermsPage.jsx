import React from 'react';
import Card from '../components/Card';

export default function TermsPage() {
  return (
    <div className="mx-auto max-w-3xl">
      <Card title="Terms of Service">
        <div className="space-y-4 text-sm text-slate-300">
          <p>
            By using FrenzyNet, you agree to follow all applicable laws and these terms. We may
            suspend or terminate accounts that violate these terms or our Acceptable Use Policy.
          </p>
          <p>
            The service is provided as-is with no warranty. We are not responsible for data loss,
            outages, or third-party disruptions beyond our reasonable control.
          </p>
          <p>
            You are responsible for keeping your account credentials secure and for all activity
            performed through your account.
          </p>
          <p>
            We may update these terms from time to time. Continued use of the service constitutes
            acceptance of the updated terms.
          </p>
        </div>
      </Card>
    </div>
  );
}
