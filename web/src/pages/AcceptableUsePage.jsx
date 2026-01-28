import React from 'react';
import Card from '../components/Card';

export default function AcceptableUsePage() {
  return (
    <div className="mx-auto max-w-3xl">
      <Card title="Acceptable Use Policy">
        <div className="space-y-4 text-sm text-slate-300">
          <p>
            FrenzyNet is built for gaming connectivity. You agree not to use the service for
            abusive, illegal, or prohibited activity.
          </p>
          <ul className="list-disc space-y-2 pl-5">
            <li>Do not use the service for torrents or other peer-to-peer file sharing.</li>
            <li>Do not use the service for scraping, crawling, or data harvesting.</li>
            <li>Do not resell or sublicense access to the service.</li>
            <li>Do not engage in abuse, attacks, or any activity that harms others.</li>
          </ul>
          <p>
            Violations may result in immediate suspension or termination of your account.
          </p>
        </div>
      </Card>
    </div>
  );
}
