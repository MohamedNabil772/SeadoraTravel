export interface InquiryPayload {
  fullName: string;
  email: string;
  phone?: string;
  destinationInterest?: string;
  dateOrGuests?: string;
  message: string;
}

export const contactApi = {
  submitInquiry: async (payload: InquiryPayload) => {
    const response = await fetch('/api/booking/api/inquiries', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(payload),
    });

    if (!response.ok) {
      throw new Error('Failed to submit inquiry');
    }

    return response.json();
  },
};
